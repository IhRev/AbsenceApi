using Absence.Application.UseCases.Holidays.Commands;
using Absence.Application.UseCases.Holidays.DTOs;
using Absence.Application.UseCases.Holidays.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("holidays")]
public class HolidaysController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("/organizations/{organizationId}/holidays")]
    public async Task<ActionResult<IEnumerable<HolidayDTO>>> Get([FromRoute] int organizationId)
    {
        var response = await _sender.Send(new GetHolidaysQuery(organizationId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateHolidayDTO holiday)
    {
        var response = await _sender.Send(new AddHolidayCommand(holiday));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditHolidayDTO holiday)
    {
        var result = await _sender.Send(new EditHolidayCommand(holiday));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var result = await _sender.Send(new DeleteHolidayCommand(id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }
}