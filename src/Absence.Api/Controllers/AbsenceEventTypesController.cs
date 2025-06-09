using Absence.Application.UseCases.AbsenceEventTypes.DTOs;
using Absence.Application.UseCases.AbsenceEventTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("absence_event_types")]
public class AbsenceEventTypesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceEventTypeDTO>>> Get()
    {
        var types = await _sender.Send(new GetAllAbsenceEventTypesQuery());
        return Ok(types);
    }
}