using Absence.Application.Common.Results;
using Absence.Application.UseCases.Users.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Users.Handlers;

internal class ChangePasswordHandler(IUserService userService, IUser user) : IRequestHandler<ChangePasswordCommand, OneOf<Success, BadRequest>>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, BadRequest>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);

        var result = await _userService.ChangePasswordAsync(user!, request.Request.OldPassword, request.Request.NewPassword);
        if (!result.Succeeded)
        {
            return new BadRequest(result.ToString());
        }

        user!.RefreshToken = null;
        user.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
        await _userService.UpdateAsync(user);

        return new Success();
    }
}