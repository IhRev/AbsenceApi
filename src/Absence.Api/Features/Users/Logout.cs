using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Identity;
using MediatR;

namespace Absence.Api.Features.Users;

public static class Logout
{
    public sealed class Command : IRequest;

    internal sealed class Handler(IUserService userService, IUser user) : IRequestHandler<Command>
    {
        private readonly IUserService _userService = userService;
        private readonly IUser _user = user;

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(_user.Id);
            user!.RefreshToken = null;
            user.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
            await _userService.UpdateAsync(user);
        }
    }
}
