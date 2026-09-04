using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Identity;
using MediatR;

namespace Absence.Api.Features.Users;

public static class GetUserDetails
{
    public sealed class Query : IRequest<UserDetails>;

    internal sealed class Handler(IUser user, IUserService userService) : IRequestHandler<Query, UserDetails>
    {
        public async Task<UserDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var identityUser = await userService.FindByIdAsync(user.Id);
            return new UserDetails
            {
                Id = identityUser!.ShortId,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName,
                Email = identityUser.Email!
            };
        }
    }
}
