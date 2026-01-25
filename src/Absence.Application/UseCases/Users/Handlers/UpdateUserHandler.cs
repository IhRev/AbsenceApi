using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Users.Handlers;

internal class UpdateUserHandler(IUserService userService, IUser user) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);
        userEntity!.FirstName = request.UserDetails.FirstName;
        userEntity.LastName = request.UserDetails.LastName;
        await userService.UpdateAsync(userEntity);
    }
}