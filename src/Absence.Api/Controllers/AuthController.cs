using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] UserCredentials credentials)
    {
        var response = await _sender.Send(new LoginUserCommand(credentials));
        return response.IsSuccess ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("refresh_token")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await _sender.Send(new RefreshTokenCommand(refreshTokenRequest));
        return response.IsSuccess ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDTO user)
    {
        var response = await _sender.Send(new AddUserCommand(user));
        return response.Match<ActionResult>(
            success => Ok(),
            error => BadRequest(error.Value)
        );
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await _sender.Send(new LogoutUserCommand());
        return Ok();
    }
}