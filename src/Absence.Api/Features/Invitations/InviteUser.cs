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
    public sealed class Command(InviteUserToOrganizationDTO invite) : IRequest<OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public InviteUserToOrganizationDTO Invite { get; } = invite;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUserService userService,
        IOrganizationAccess organizationAccess,
        IUser user
    ) : IRequestHandler<Command, OneOf<Success, NotFound, BadRequest, AccessDenied>>
    {
        public async Task<OneOf<Success, NotFound, BadRequest, AccessDenied>> Handle(Command request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireAdminAsync(request.Invite.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out var denied))
            {
                return denied.Match<OneOf<Success, NotFound, BadRequest, AccessDenied>>(
                    notFound => notFound,
                    accessDenied => accessDenied);
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
