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
    public async Task<OneOf<Success, BadRequest>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);

        var result = await userService.ChangePasswordAsync(userEntity!, request.Request.OldPassword, request.Request.NewPassword);
        if (!result.Succeeded)
        {
            return new BadRequest(result.Errors.First().Description);
        }

        userEntity!.RefreshToken = null;
        userEntity.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
        await userService.UpdateAsync(userEntity);

        return new Success();
    }
}