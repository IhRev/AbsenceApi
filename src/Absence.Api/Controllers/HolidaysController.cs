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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HolidayDTO>>> Get()
    {
        var holidays = await _sender.Send(new GetHolidaysQuery());
        return Ok(holidays);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateHolidayDTO holiday)
    {
        int id = await _sender.Send(new AddHolidayCommand(holiday));
        return Ok(id);
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditHolidayDTO holiday)
    {
        var result = await _sender.Send(new EditHolidayCommand(holiday));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var result = await _sender.Send(new DeleteHolidayCommand(id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }
}