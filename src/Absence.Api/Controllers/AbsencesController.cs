using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Application.UseCases.Absences.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("absences")]
public class AbsencesController(
    ILogger<AbsencesController> logger,
    ISender sender
) : ControllerBase
{
    private readonly ILogger<AbsencesController> _logger = logger;
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceDTO>>> Get()
    {
        try
        {
            var absences = await _sender.Send(new GetUserAbsencesQuery("id"));
            return Ok(absences);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    } 

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateAbsenceDTO absence)
    {
        try
        {
            int id = await _sender.Send(new AddAbsenceCommand(absence, "id"));
            return Ok(id);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
} 