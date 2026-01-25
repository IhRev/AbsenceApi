using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Users.Handlers;

internal class LogoutUserHandler(IUserService userService, IUser user) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        userEntity!.RefreshToken = null;
        userEntity.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
        await userService.UpdateAsync(userEntity);
    }
}