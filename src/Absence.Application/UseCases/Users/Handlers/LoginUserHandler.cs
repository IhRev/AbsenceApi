using Absence.Application.Common.Responses;
using Absence.Application.UseCases.Users.Commands;
using Absence.Domain.Interfaces;
using MediatR;

namespace Absence.Application.UseCases.Users.Handlers;

internal class LoginUserHandler(IUserService userRepository, IJwtService jwtService) : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserService _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Credentials.Email);
        if (user == null || await _userRepository.CheckPasswordAsync(user, request.Credentials.Password))
        {
            return AuthResponse.Fail("Incorrect email or password");
        }

        var token = _jwtService.GenerateToken(user);

        return AuthResponse.Success(token);
    }
}