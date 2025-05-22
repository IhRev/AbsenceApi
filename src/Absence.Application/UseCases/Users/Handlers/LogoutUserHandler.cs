using Absence.Application.UseCases.Users.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.DTOs;
using Absence.Domain.Interfaces;
using Absence.Application.Common.Interfaces;

namespace Absence.Application.UseCases.Users.Handlers;

internal class LogoutUserHandler(IUserService userService, IUser user) : IRequestHandler<LogoutUserCommand, OneOf<Success, NotFound>>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, NotFound>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        if (user is null)
        {
            return new NotFound();
        }

        user.RefreshToken = null;
        user.RefreshTokenExpires = DateTimeOffset.MinValue;
        await _userService.UpdateAsync(user);

        return new Success();
    }
}