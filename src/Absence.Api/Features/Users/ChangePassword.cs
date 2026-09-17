using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public class ChangePasswordRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string OldPassword { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string NewPassword { get; set; }
}

public static class ChangePassword
{
    public sealed class Command(ChangePasswordRequest request) : IRequest<OneOf<Success, BadRequest, Unauthorized>>
    {
        public ChangePasswordRequest Request { get; } = request;
    }

    internal sealed class Handler(IUserService userService, IUser currentUser) : IRequestHandler<Command, OneOf<Success, BadRequest, Unauthorized>>
    {
        public async Task<OneOf<Success, BadRequest, Unauthorized>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(currentUser.Id);
            if (user is null)
            {
                return new Unauthorized();
            }

            var result = await userService.ChangePasswordAsync(user, request.Request.OldPassword, request.Request.NewPassword);
            if (!result.Succeeded)
            {
                return new BadRequest(result.Errors.First().Description);
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
            await userService.UpdateAsync(user);

            return new Success();
        }
    }
}
