using Absence.Application.Common.Responses;
using Absence.Application.UseCases.Users.Commands;
using Absence.Domain.Interfaces;
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
        if (user == null || !await _userService.CheckPasswordAsync(user, request.Credentials.Password))
        {
            return AuthResponse.Fail("Incorrect email or password");
        }

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = await _refreshTokenService.GenerateToken(user, cancellationToken);

        return AuthResponse.Success(accessToken, refreshToken);
    }
}