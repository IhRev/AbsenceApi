using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Holidays;

[Authorize]
[ApiController]
[Route("holidays")]
public class HolidaysController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("/organizations/{organizationId}/holidays")]
    public async Task<ActionResult<IEnumerable<HolidayDTO>>> Get([FromRoute] int organizationId, [FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate)
    {
        var response = await _sender.Send(new GetHolidays.Query(organizationId, startDate, endDate));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateHolidayDTO holiday)
    {
        var response = await _sender.Send(new AddHoliday.Command(holiday));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditHolidayDTO holiday)
    {
        var result = await _sender.Send(new EditHoliday.Command(holiday));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid(),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var result = await _sender.Send(new DeleteHoliday.Command(id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }
}
