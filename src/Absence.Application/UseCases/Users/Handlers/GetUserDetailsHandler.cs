using Absence.Application.Common.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Queries;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Users.Handlers;

internal class GetUserDetailsHandler(IUser user, IUserService userService, IMapper mapper) : IRequestHandler<GetUserDetailsQuery, UserDetails>
{
    private readonly IUser _user = user;
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDetails> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        return _mapper.Map<UserDetails>(user);
    }
}