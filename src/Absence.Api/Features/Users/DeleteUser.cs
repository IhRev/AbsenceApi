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
    public sealed class Command(DeleteUserRequest request) : IRequest<OneOf<Success, BadRequest>>
    {
        public DeleteUserRequest Request { get; } = request;
    }

    internal sealed class Handler(
        IUserService userService,
        IUser user,
        AbsenceContext db
    ) : IRequestHandler<Command, OneOf<Success, BadRequest>>
    {
        public async Task<OneOf<Success, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identityUser = await userService.FindByIdAsync(user.Id);

            if (!await userService.CheckPasswordAsync(identityUser!, request.Request.Password))
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

            await userService.DeleteAsync(identityUser!);

            return new Success();
        }
    }
}
