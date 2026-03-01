using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.UseCases.Events.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("organizations/{organizationId}/events")]
public class EventsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDTO>>> Get(
        [FromRoute] int organizationId, 
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate
    ) => Ok(await sender.Send(new GetEventsQuery(organizationId, startDate, endDate)));

    [HttpPost]
    public async Task<ActionResult<int>> Add(
        [FromRoute] int organizationId, 
        [FromBody] CreateEventDTO @event
    ) => Ok(await sender.Send(new AddEventCommand(organizationId, @event)));

    [HttpPut]
    public async Task<ActionResult> Edit([FromRoute] int organizationId, [FromBody] EditEventDTO @event)
    {
        var result = await sender.Send(new EditEventCommand(organizationId, @event));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int organizationId, [FromRoute] int id)
    {
        var result = await sender.Send(new DeleteEventCommand(organizationId, id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }
}