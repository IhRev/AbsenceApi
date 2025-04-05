using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ILogger<AuthController> logger, ISender sender) : ControllerBase
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly ISender _sender = sender;

    [HttpPost("login")]
    public async Task<ActionResult<int>> Login([FromBody] UserCredentialsDTO credentials)
    {
        try
        {
            var response = await _sender.Send(new LoginUserCommand(credentials));
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
}