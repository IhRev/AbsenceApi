using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Domain.Entities;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Handlers;

internal class AddUserHandler(
    IUserService userRepository
) : IRequestHandler<AddUserCommand, OneOf<Success, Error<string>>>
{
    public async Task<OneOf<Success, Error<string>>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserEntity()
        {
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Email = request.User.Credentials.Email,
            UserName = request.User.Credentials.Email,
        };

        var result = await userRepository.CreateAsync(user, request.User.Credentials.Password);
        if (!result.Succeeded)
        {
            return new Error<string>(result.ToString());
        }

        return new Success();
    }
}