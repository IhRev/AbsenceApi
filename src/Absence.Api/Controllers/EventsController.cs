using Absence.Application.UseCases.Holidays.Commands;
using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Application.UseCases.Holidays.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("events")]
public class EventsController(ISender sender) : ControllerBase
{
    [HttpGet("/organizations/{organizationId}/events")]
    public async Task<ActionResult<IEnumerable<HolidayDTO>>> Get([FromRoute] int organizationId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var response = await sender.Send(new GetEventsQuery(organizationId, startDate, endDate));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            accessDenied => Forbid()
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateHolidayDTO @event)
    {
        var response = await sender.Send(new AddEventCommand(@event));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            accessDenied => Forbid()
        );
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditEventDTO @event)
    {
        var result = await sender.Send(new EditEventCommand(@event));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var result = await sender.Send(new DeleteEventCommand(id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }
}