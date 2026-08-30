using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Handlers;

internal class LoginUserHandler(
    IUserService userService, 
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService
) : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserService _userService = userService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
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