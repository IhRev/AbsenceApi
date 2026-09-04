using System.ComponentModel.DataAnnotations;
using Absence.Infrastructure.Entities;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Users;

public class RegisterDTO
{
    [Required(AllowEmptyStrings = false)]
    public string FirstName { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    public string LastName { get; set; } = null!;
    [Required]
    public required UserCredentials Credentials { get; set; }
}

public static class Register
{
    public sealed class Command(RegisterDTO user) : IRequest<OneOf<Success, Error<string>>>
    {
        public RegisterDTO User { get; } = user;
    }

    internal sealed class Handler(IUserService userRepository) : IRequestHandler<Command, OneOf<Success, Error<string>>>
    {
        private readonly IUserService _userRepository = userRepository;

        public async Task<OneOf<Success, Error<string>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new UserEntity()
            {
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                Email = request.User.Credentials.Email,
                UserName = request.User.Credentials.Email,
            };

            var result = await _userRepository.CreateAsync(user, request.User.Credentials.Password);
            if (!result.Succeeded)
            {
                return new Error<string>(result.ToString());
            }

            return new Success();
        }
    }
}
