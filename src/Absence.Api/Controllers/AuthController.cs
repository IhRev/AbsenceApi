using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("login")]
    public async Task<ActionResult<int>> Login([FromBody] UserCredentialsDTO credentials)
    {
        var response = await _sender.Send(new LoginUserCommand(credentials));
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<int>> Register([FromBody] RegisterDTO user)
    {
        var id = await _sender.Send(new AddUserCommand(user));
        return Ok(id);
    }
}