using Absence.Application.Common.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Queries;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Handlers;

internal class GetUserDetailsHandler(
    IUser user, 
    IUserService userService, 
    IMapper mapper
) : IRequestHandler<GetUserDetailsQuery, OneOf<UserDetails, NotFound>>
{
    public async Task<OneOf<UserDetails, NotFound>> Handle(
        GetUserDetailsQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        if (userEntity == null )
        {
            return new NotFound();
        }
        return mapper.Map<UserDetails>(userEntity);
    }
}