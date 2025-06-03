using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Users.Handlers;

internal class LogoutUserHandler(IUserService userService, IUser user) : IRequestHandler<LogoutUserCommand>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        user!.RefreshToken = null;
        user.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
        await _userService.UpdateAsync(user);
    }
}