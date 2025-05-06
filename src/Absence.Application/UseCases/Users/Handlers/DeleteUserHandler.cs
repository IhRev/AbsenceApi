using Absence.Application.UseCases.Users.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Interfaces;
using Absence.Domain.Interfaces;
using Absence.Application.Common.Results;

namespace Absence.Application.UseCases.Users.Handlers;

internal class DeleteUserHandler(IUserService userService, IUser user) : IRequestHandler<DeleteUserCommand, OneOf<Success, NotFound, BadRequest>>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, NotFound, BadRequest>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        if (user is null)
        {
            return new NotFound();
        }

        if (!await _userService.CheckPasswordAsync(user, request.Request.Password))
        {
            return new BadRequest();
        }

        await _userService.DeleteAsync(user);

        return new Success();
    }
}