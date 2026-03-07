using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Handlers;

internal class LogoutUserHandler(IUserService userService, IUser user) 
    : IRequestHandler<LogoutUserCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(LogoutUserCommand request, CancellationToken cancellationToken = default)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        if (userEntity == null)
        {
            return new NotFound();
        }

        userEntity.RefreshToken = null;
        userEntity.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
        await userService.UpdateAsync(userEntity);

        return new Success();
    }
}