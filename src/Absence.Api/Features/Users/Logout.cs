using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public static class Logout
{
    public sealed class Command : IRequest<OneOf<Success, Unauthorized>>;

    internal sealed class Handler(IUserService userService, IUser currentUser) : IRequestHandler<Command, OneOf<Success, Unauthorized>>
    {
        public async Task<OneOf<Success, Unauthorized>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByIdAsync(currentUser.Id);
            if (user is null)
            {
                return new Unauthorized();
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiresAt = DateTimeOffset.MinValue;
            await userService.UpdateAsync(user);

            return new Success();
        }
    }
}
