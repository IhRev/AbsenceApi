using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
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
    private readonly IRepository<OrganizationUserInvitationEntity> _organizationUserInvitationRepository = organizationUserInvitationRepository;
    private readonly IOrganizationUsersRepository _organizationUserRepository = organizationUserRepository;
    private readonly IUser _user = user;
    private readonly IMapper _mapper = mapper;

    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _organizationUserInvitationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invitation is null)
        {
            return new NotFound();
        }

        if (invitation.UserId != _user.ShortId)
        {
            return new AccessDenied();
        }

        if (request.Accespted)
        {
            var organizationUser = _mapper.Map<OrganizationUserEntity>(invitation);
            await _organizationUserRepository.InsertAsync(organizationUser, cancellationToken);
            await _organizationUserRepository.SaveAsync(cancellationToken);
        }

        _organizationUserInvitationRepository.Delete(invitation);
        await _organizationUserInvitationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}