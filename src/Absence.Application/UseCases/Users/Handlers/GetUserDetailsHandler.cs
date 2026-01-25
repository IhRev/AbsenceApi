using Absence.Application.Common.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Queries;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Users.Handlers;

internal class GetUserDetailsHandler(
    IUser user, 
    IUserService userService, 
    IMapper mapper
) : IRequestHandler<GetUserDetailsQuery, UserDetails>
{
    public async Task<UserDetails> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        return mapper.Map<UserDetails>(userEntity);
    }
}