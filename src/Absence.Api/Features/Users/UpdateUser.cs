using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Identity;
using MediatR;

namespace Absence.Api.Features.Users;

public static class UpdateUser
{
    public sealed class Command(UserDetails userDetails) : IRequest
    {
        public UserDetails UserDetails { get; } = userDetails;
    }

    internal sealed class Handler(IUserService userService, IUser user) : IRequestHandler<Command>
    {
        private readonly IUserService _userService = userService;
        private readonly IUser _user = user;

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(_user.Id);
            user!.FirstName = request.UserDetails.FirstName;
            user.LastName = request.UserDetails.LastName;
            await _userService.UpdateAsync(user);
        }
    }
}
