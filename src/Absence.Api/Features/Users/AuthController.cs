using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Users;

[ApiController]
[Route("auth")]
public class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokens>> Login([FromBody] UserCredentials credentials)
    {
        var result = await _sender.Send(new Login.Command(credentials));
        return result.Match<ActionResult>(
            tokens => Ok(tokens),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPost("refresh_token")]
    public async Task<ActionResult<AuthTokens>> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var result = await _sender.Send(new RefreshToken.Command(refreshTokenRequest));
        return result.Match<ActionResult>(
            tokens => Ok(tokens),
            // The handler's failure is an authentication failure on an anonymous endpoint.
            badRequest => Unauthorized(badRequest.Message)
        );
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDTO user)
    {
        var response = await _sender.Send(new Register.Command(user));
        return response.Match<ActionResult>(
            success => Ok(),
            error => BadRequest(error.Value)
        );
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var result = await _sender.Send(new Logout.Command());
        return result.Match<ActionResult>(
            success => Ok(),
            unauthorized => Unauthorized()
        );
    }
}
