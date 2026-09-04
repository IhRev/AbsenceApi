using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Entities;
using Absence.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Invitations;

public class InviteUserToOrganizationDTO
{
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public required string UserEmail { get; set; }
    [Required]
    public required int OrganizationId { get; set; }
}

public static class InviteUser
{
    public sealed class Command(InviteUserToOrganizationDTO invite) : IRequest<OneOf<Success, BadRequest, AccessDenied>>
    {
        public InviteUserToOrganizationDTO Invite { get; } = invite;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUserService userService,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var organization = await db.Organizations.FirstOrDefaultAsync(
                _ => _.Id == request.Invite.OrganizationId,
                cancellationToken);
            if (organization is null)
            {
                return new BadRequest($"Organization with id {request.Invite.OrganizationId} doesn't exist.");
            }

            var inviterOrganization = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.OrganizationId == request.Invite.OrganizationId && _.UserId == user.ShortId,
                cancellationToken);
            if (inviterOrganization is null || !inviterOrganization.IsAdmin)
            {
                return new AccessDenied();
            }

            var invitedUser = await userService.FindByEmailAsync(request.Invite.UserEmail);
            if (invitedUser is null)
            {
                return new BadRequest($"User with email {request.Invite.UserEmail} doesn't exist.");
            }

            var invitedUserOrganization = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.OrganizationId == request.Invite.OrganizationId && _.UserId == invitedUser.ShortId,
                cancellationToken);
            if (invitedUserOrganization is not null)
            {
                return new BadRequest($"Invited user already belongs to organization.");
            }

            var invitation = await db.OrganizationUserInvitations.FirstOrDefaultAsync(
                _ => _.OrganizationId == request.Invite.OrganizationId && _.Invited == invitedUser.ShortId,
                cancellationToken);
            if (invitation is not null)
            {
                return new BadRequest("Invitation already sent.");
            }

            db.OrganizationUserInvitations.Add(new OrganizationUserInvitationEntity()
            {
                Invited = invitedUser.ShortId,
                Inviter = user.ShortId,
                OrganizationId = request.Invite.OrganizationId
            });
            await db.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}
