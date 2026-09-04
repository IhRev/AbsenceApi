using System.ComponentModel.DataAnnotations;
using Absence.Infrastructure.Identity;
using MediatR;

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
    public sealed class Command(UserCredentials credentials) : IRequest<AuthResponse>
    {
        public UserCredentials Credentials { get; } = credentials;
    }

    internal sealed class Handler(
        IUserService userService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    ) : IRequestHandler<Command, AuthResponse>
    {
        private readonly IUserService _userService = userService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

        public async Task<AuthResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByEmailAsync(request.Credentials.Email);
            if (user == null)
            {
                return AuthResponse.Fail("Incorrect email or password");
            }

            if (await _userService.IsLockedOutAsync(user))
            {
                return AuthResponse.Fail("Account is locked. Try again later.");
            }

            if (!await _userService.CheckPasswordAsync(user, request.Credentials.Password))
            {
                await _userService.AccessFailedAsync(user);
                if (await _userService.IsLockedOutAsync(user))
                {
                    return AuthResponse.Fail("Account is locked. Try again later.");
                }

                return AuthResponse.Fail("Incorrect email or password");
            }

            await _userService.ResetAccessFailedCountAsync(user);

            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = await _refreshTokenService.GenerateToken(user, cancellationToken);

            return AuthResponse.Success(accessToken, refreshToken);
        }
    }
}
