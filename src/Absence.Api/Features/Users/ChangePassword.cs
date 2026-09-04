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
    public sealed class Command(ChangePasswordRequest request) : IRequest<OneOf<Success, BadRequest>>
    {
        public ChangePasswordRequest Request { get; } = request;
    }

    internal sealed class Handler(IUserService userService, IUser user) : IRequestHandler<Command, OneOf<Success, BadRequest>>
    {
        private readonly IUserService _userService = userService;
        private readonly IUser _user = user;

        public async Task<OneOf<Success, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(_user.Id);

            var result = await _userService.ChangePasswordAsync(user!, request.Request.OldPassword, request.Request.NewPassword);
            if (!result.Succeeded)
            {
                return new BadRequest(result.Errors.First().Description);
            }

            user!.RefreshToken = null;
            user.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
            await _userService.UpdateAsync(user);

            return new Success();
        }
    }
}
