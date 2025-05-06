using Absence.Application.Common.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Users.Queries;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Handlers;

internal class GetUserDetailsHandler(IUser user, IUserService userService, IMapper mapper) : IRequestHandler<GetUserDetailsQuery, OneOf<Success<UserDetails>, NotFound>>
{
    private readonly IUser _user = user;
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    public async Task<OneOf<Success<UserDetails>, NotFound>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        if (user is null)
        {
            return new NotFound();
        }

        return new Success<UserDetails>(_mapper.Map<UserDetails>(user));
    }
}