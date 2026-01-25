using Absence.Application.UseCases.Users.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Users.Handlers;

internal class DeleteUserHandler(IUserService userService, IUser user) : IRequestHandler<DeleteUserCommand, OneOf<Success, BadRequest>>
{
    public async Task<OneOf<Success, BadRequest>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByIdAsync(user.Id);

        if (!await userService.CheckPasswordAsync(userEntity!, request.Request.Password))
        {
            return new BadRequest("Password is invalid.");
        }

        await userService.DeleteAsync(userEntity!);

        return new Success();
    }
}