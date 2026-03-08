using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Invitations.Handlers;

internal class AcceptInvitationHandler(
    IRepository<OrganizationUserInvitationEntity> organizationUserInvitationRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IRepository<OrganizationRoleEntity> organizationRoleRepository,
    IUser user
) : IRequestHandler<AcceptInvitationCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        AcceptInvitationCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var invitation = await organizationUserInvitationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invitation is null || invitation.Invited != user.ShortId)
        {
            return new NotFound();
        }

        if (request.Accespted)
        {
            await CreateViewerRole(invitation.OrganizationId, cancellationToken);
        }
        await DeleteInvitation(invitation, cancellationToken);

        return new Success();
    }

    private async Task CreateViewerRole(int organizationId, CancellationToken cancellationToken = default)
    {
        var role = await organizationRoleRepository.GetFirstOrDefaultAsync(
            new RoleSpec(organizationId, SystemRoleNames.VIEWER),
            cancellationToken
        );
         
        await userOrganizationRoleRepository.InsertAsync(
            new()
            {
                OrganizationRoleId = role!.Id,
                UserId = user.ShortId,
                OrganizationId = organizationId
            }, 
            cancellationToken
        );
        await userOrganizationRoleRepository.SaveAsync(cancellationToken);
    }

    private Task DeleteInvitation(
        OrganizationUserInvitationEntity invitation, 
        CancellationToken cancellationToken = default
    )
    {
        organizationUserInvitationRepository.Delete(invitation);
        return organizationUserInvitationRepository.SaveAsync(cancellationToken);
    }
}