using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public class DeleteUserRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}

public static class DeleteUser
{
    public sealed class Command(DeleteUserRequest request) : IRequest<OneOf<Success, BadRequest, Unauthorized>>
    {
        public DeleteUserRequest Request { get; } = request;
    }

    internal sealed class Handler(
        IUserService userService,
        IUser user,
        AbsenceContext db
    ) : IRequestHandler<Command, OneOf<Success, BadRequest, Unauthorized>>
    {
        public async Task<OneOf<Success, BadRequest, Unauthorized>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identityUser = await userService.FindByIdAsync(user.Id);
            if (identityUser is null)
            {
                return new Unauthorized();
            }

            if (!await userService.CheckPasswordAsync(identityUser, request.Request.Password))
            {
                return new BadRequest("Password is invalid.");
            }

            var ownedOrganization = await db.Organizations.FirstOrDefaultAsync(
                _ => _.OwnerId == user.ShortId,
                cancellationToken);
            if (ownedOrganization is not null)
            {
                return new BadRequest("Transfer or delete owned organizations first.");
            }

            // Invitations this user received cascade, but the ones they sent are NoAction to avoid
            // two cascade paths into the same table, so they have to go before the user does.
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            await db.OrganizationUserInvitations
                .Where(_ => _.Inviter == user.ShortId)
                .ExecuteDeleteAsync(cancellationToken);

            var deletion = await userService.DeleteAsync(identityUser);
            if (!deletion.Succeeded)
            {
                return new BadRequest(deletion.Errors.First().Description);
            }

            await transaction.CommitAsync(cancellationToken);

            return new Success();
        }
    }
}
