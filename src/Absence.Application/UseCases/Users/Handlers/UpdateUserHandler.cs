using Absence.Application.UseCases.Users.Commands;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Domain.Interfaces;
using Absence.Application.Common.Interfaces;

namespace Absence.Application.UseCases.Users.Handlers;

internal class UpdateUserHandler(IUserService userService, IUser user) : IRequestHandler<UpdateUserCommand, OneOf<Success, NotFound>>
{
    private readonly IUserService _userService = userService;
    private readonly IUser _user = user;

    public async Task<OneOf<Success, NotFound>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByIdAsync(_user.Id);
        if (user is null)
        {
            return new NotFound();
        }

        user.FirstName = request.UserDetails.FirstName;
        user.LastName = request.UserDetails.LastName;
        await _userService.UpdateAsync(user);

        return new Success();
    }
}