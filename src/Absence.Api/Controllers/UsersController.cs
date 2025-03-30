using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController(
    ILogger<AbsencesController> logger,
    ISender sender
) : ControllerBase
{
    private readonly ILogger<AbsencesController> _logger = logger;
    private readonly ISender _sender = sender;

    [HttpGet]
    public ActionResult<IEnumerable<AbsenceDTO>> Get()
    {
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] CreateUserDTO user)
    {
        try
        {
            var id = _sender.Send(new AddUserCommand(user));
            return Ok(id);
        }
        catch (Exception)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
}