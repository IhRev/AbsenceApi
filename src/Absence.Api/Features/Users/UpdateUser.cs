using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public class UpdateUserRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string FirstName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string LastName { get; set; }
}

public static class UpdateUser
{
    public sealed class Command(UpdateUserRequest userDetails) : IRequest<OneOf<Success, Unauthorized>>
    {
        public UpdateUserRequest UserDetails { get; } = userDetails;
    }

    internal sealed class Handler(IUserService userService, IUser currentUser) : IRequestHandler<Command, OneOf<Success, Unauthorized>>
    {
        public async Task<OneOf<Success, Unauthorized>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(currentUser.Id);
            if (user is null)
            {
                return new Unauthorized();
            }

            user.FirstName = request.UserDetails.FirstName;
            user.LastName = request.UserDetails.LastName;
            (await userService.UpdateAsync(user)).EnsureSucceeded("Updating the user profile");

            return new Success();
        }
    }
}
