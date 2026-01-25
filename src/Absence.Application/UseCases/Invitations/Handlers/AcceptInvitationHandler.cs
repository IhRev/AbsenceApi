using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Repositories;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Handlers;

internal class AcceptInvitationHandler(
    IRepository<OrganizationUserInvitationEntity> organizationUserInvitationRepository,
    IOrganizationUsersRepository organizationUserRepository,
    IUser user,
    IMapper mapper
) : IRequestHandler<AcceptInvitationCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await organizationUserInvitationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invitation is null)
        {
            return new NotFound();
        }

        if (invitation.Invited != user.ShortId)
        {
            return new AccessDenied();
        }

        if (request.Accespted)
        {
            var organizationUser = mapper.Map<OrganizationUserEntity>(invitation);
            await organizationUserRepository.InsertAsync(organizationUser, cancellationToken);
            await organizationUserRepository.SaveAsync(cancellationToken);
        }

        organizationUserInvitationRepository.Delete(invitation);
        await organizationUserInvitationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}