using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;

namespace Absence.Application.UseCases.Users.Handlers;

internal class UpdateUserHandler(IUserService userService, IUser user) : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        user!.FirstName = request.UserDetails.FirstName;
        user.LastName = request.UserDetails.LastName;
        await _userService.UpdateAsync(user);
    }
}