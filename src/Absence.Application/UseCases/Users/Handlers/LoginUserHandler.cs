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
    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await userService.FindByEmailAsync(request.Credentials.Email);
        if (userEntity == null || !await userService.CheckPasswordAsync(userEntity, request.Credentials.Password))
        {
            return AuthResponse.Fail("Incorrect email or password");
        }

        var accessToken = jwtService.GenerateToken(userEntity);
        var refreshToken = await refreshTokenService.GenerateToken(userEntity, cancellationToken);

        return AuthResponse.Success(accessToken, refreshToken);
    }
}