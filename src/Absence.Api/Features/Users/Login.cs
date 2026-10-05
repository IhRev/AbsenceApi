using System.ComponentModel.DataAnnotations;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;

namespace Absence.Api.Features.Users;

public class UserCredentials
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}

public static class Login
{
    public sealed class Command(UserCredentials credentials) : IRequest<OneOf<AuthTokens, BadRequest>>
    {
        public UserCredentials Credentials { get; } = credentials;
    }

    internal sealed class Handler(
        IUserService userService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    ) : IRequestHandler<Command, OneOf<AuthTokens, BadRequest>>
    {
        public async Task<OneOf<AuthTokens, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userService.FindByEmailAsync(request.Credentials.Email);
            if (user == null)
            {
                return new BadRequest("Incorrect email or password");
            }

            if (await userService.IsLockedOutAsync(user))
            {
                return new BadRequest("Account is locked. Try again later.");
            }

            if (!await userService.CheckPasswordAsync(user, request.Credentials.Password))
            {
                await userService.AccessFailedAsync(user);
                if (await userService.IsLockedOutAsync(user))
                {
                    return new BadRequest("Account is locked. Try again later.");
                }

                return new BadRequest("Incorrect email or password");
            }

            await userService.ResetAccessFailedCountAsync(user);

            return new AuthTokens
            {
                AccessToken = jwtService.GenerateToken(user),
                RefreshToken = await refreshTokenService.GenerateToken(user, cancellationToken)
            };
        }
    }
}
