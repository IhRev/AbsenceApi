using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Identity;
using AutoMapper;
using MediatR;

namespace Absence.Api.Features.Users;

public static class GetUserDetails
{
    public sealed class Query : IRequest<UserDetails>;

    internal sealed class Handler(IUser user, IUserService userService, IMapper mapper) : IRequestHandler<Query, UserDetails>
    {
        private readonly IUser _user = user;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        public async Task<UserDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(_user.Id);
            return _mapper.Map<UserDetails>(user);
        }
    }
}
