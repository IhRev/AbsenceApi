using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Absences;

[Authorize]
[ApiController]
[Route("absences")]
public class AbsencesController(ISender sender) : ControllerBase
{
    [HttpGet("/organizations/{organizationId}/absences")]
    public async Task<ActionResult<IEnumerable<AbsenceDTO>>> Get(
        [FromRoute] int organizationId,
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate,
        [FromQuery] List<int>? userIds)
    {
        if (userIds is { Count: > 0 })
        {
            var filtered = await sender.Send(new GetUsersAbsences.Query(startDate, endDate, organizationId, userIds));
            return filtered.Match<ActionResult>(
                success => Ok(success.Value),
                notFound => NotFound(),
                accessDenied => Forbid()
            );
        }

        var response = await sender.Send(new GetUserAbsences.Query(startDate, endDate, organizationId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            notFound => NotFound()
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateAbsenceDTO absence)
    {
        var response = await sender.Send(new AddAbsence.Command(absence));
        return response.Match<ActionResult>(
            persisted => Created($"/absences/{persisted.AbsenceId}", persisted.AbsenceId),
            queued => Accepted(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditAbsenceDTO absence)
    {
        var result = await sender.Send(new EditAbsence.Command(absence));
        return result.Match<ActionResult>(
            persisted => Ok(),
            queued => Accepted(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var result = await sender.Send(new DeleteAbsence.Command(id));
        return result.Match<ActionResult>(
            persisted => NoContent(),
            queued => Accepted(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpGet("/organizations/{organizationId}/absences/events")]
    public async Task<ActionResult<IEnumerable<AbsenceEventDTO>>> GetEvents([FromRoute] int organizationId)
    {
        var response = await sender.Send(new GetAbsenceEvents.Query(organizationId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpPost("events/{eventId}")]
    public async Task<ActionResult> Respond([FromRoute] int eventId, [FromQuery] bool accepted)
    {
        var response = await sender.Send(new RespondAbsenceEvent.Command(eventId, accepted));
        return response.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid(),
            badRequest => BadRequest(badRequest.Message)
        );
    }
}
