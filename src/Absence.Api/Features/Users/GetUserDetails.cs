using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public static class GetUserDetails
{
    public sealed class Query : IRequest<OneOf<Success<UserDetails>, Unauthorized>>;

    internal sealed class Handler(IUser currentUser, IUserService userService)
        : IRequestHandler<Query, OneOf<Success<UserDetails>, Unauthorized>>
    {
        public async Task<OneOf<Success<UserDetails>, Unauthorized>> Handle(Query request, CancellationToken cancellationToken)
        {
            var identityUser = await userService.FindByIdAsync(currentUser.Id);
            if (identityUser is null)
            {
                return new Unauthorized();
            }

            return new Success<UserDetails>(new UserDetails
            {
                Id = identityUser.ShortId,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName,
                Email = identityUser.Email!
            });
        }
    }
}
