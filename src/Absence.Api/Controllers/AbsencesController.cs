using Absence.Application.UseCases.Absences.Commands;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Absences.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("absences")]
public class AbsencesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [Route("organizations")]
    [HttpGet("{organizationId}/absences")]
    public async Task<ActionResult<IEnumerable<AbsenceDTO>>> Get([FromRoute] int organizationId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var response = await _sender.Send(new GetUserAbsencesQuery(startDate, endDate, organizationId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    } 

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateAbsenceDTO absence)
    {
        var response = await _sender.Send(new AddAbsenceCommand(absence));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditAbsenceDTO absence)
    {
        var result = await _sender.Send(new EditAbsenceCommand(absence));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var result = await _sender.Send(new DeleteAbsenceCommand(id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }
} 