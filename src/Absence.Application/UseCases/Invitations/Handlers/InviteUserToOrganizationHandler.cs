using Absence.Application.Common.Results;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Entities;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Application.Identity;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;

namespace Absence.Application.UseCases.Invitations.Handlers;

public class InviteUserToOrganizationHandler(
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IRepository<OrganizationUserInvitationEntity> invitationRepository,
    IUserService userService,
    IUser user
) : IRequestHandler<InviteUserToOrganizationCommand, OneOf<Success, BadRequest>>
{
    public async Task<OneOf<Success, BadRequest>> Handle(
        InviteUserToOrganizationCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var invitedUser = await userService.FindByEmailAsync(request.Invite.UserEmail);
        if (invitedUser is null)
        {
            return new BadRequest($"User with email {request.Invite.UserEmail} doesn't exist.");
        }

        var userAlreadyBelongsToOrganization = await userOrganizationRoleRepository.AnyAsync(
            new UserRoleSpec(invitedUser.ShortId, request.OrganizationId),
            cancellationToken
        );
        if (userAlreadyBelongsToOrganization)
        {
            return new BadRequest($"Invited user already belongs to organization.");
        }

        var invitationAlreadySend = await invitationRepository.AnyAsync(
            new InvitationSpec(invitedUser.ShortId, request.OrganizationId),
            cancellationToken
        );
        if (invitationAlreadySend)
        {
            return new BadRequest("Invitation already sent.");
        }

        await invitationRepository.InsertAsync(
            new()
            {
                Invited = invitedUser.ShortId,
                Inviter = user.ShortId,
                OrganizationId = request.OrganizationId
            },
            cancellationToken
        );
        await invitationRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}