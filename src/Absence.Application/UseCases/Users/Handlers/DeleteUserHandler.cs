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
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, BadRequest>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);

        if (!await _userService.CheckPasswordAsync(user!, request.Request.Password))
        {
            return new BadRequest("Password is invalid.");
        }

        await _userService.DeleteAsync(user!);

        return new Success();
    }
}