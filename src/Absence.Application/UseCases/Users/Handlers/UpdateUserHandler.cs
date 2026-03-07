using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Users.Handlers;

internal class UpdateUserHandler(IUserService userService, IUser user) 
    : IRequestHandler<UpdateUserCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        if (userEntity == null)
        {
            return new NotFound();
        }

        userEntity!.FirstName = request.UserDetails.FirstName;
        userEntity.LastName = request.UserDetails.LastName;
        await userService.UpdateAsync(userEntity);
        return new Success();
    }
}