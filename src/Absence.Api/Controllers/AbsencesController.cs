using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Absences.Commands;
using Absence.Application.UseCases.Absences.DTOs;
using Absence.Application.UseCases.Absences.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("absences")]
public class AbsencesController(ISender sender, IUser user) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly IUser _user = user;

    [HttpGet("/organizations/{organizationId}/absences")]
    public async Task<ActionResult<IEnumerable<AbsenceDTO>>> Get([FromRoute] int organizationId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var response = await _sender.Send(new GetUserAbsencesQuery(startDate, endDate, organizationId, _user.ShortId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpGet("/organizations/{organizationId}/users/{userId}/absences")]
    public async Task<ActionResult<IEnumerable<AbsenceDTO>>> GetByUserId([FromRoute] int organizationId, [FromRoute] int userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var response = await _sender.Send(new GetUserAbsencesQuery(startDate, endDate, organizationId, userId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateAbsenceDTO absence)
    {
        var response = await _sender.Send(new AddAbsenceCommand(absence));
        return response.Match<ActionResult>(
            successCreated => Ok(successCreated.Value),
            successRequested => Ok(new { Message = successRequested.Value }),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPut]
    public async Task<ActionResult<string>> Edit([FromBody] EditAbsenceDTO absence)
    {
        var result = await _sender.Send(new EditAbsenceCommand(absence));
        return result.Match<ActionResult>(
            success => Ok(new { Message = success.Value }),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> Delete([FromRoute] int id)
    {
        var result = await _sender.Send(new DeleteAbsenceCommand(id));
        return result.Match<ActionResult>(
            success => Ok(new { Message = success.Value }),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpGet("/organizations/{organizationId}/absences/events")]
    public async Task<ActionResult<IEnumerable<AbsenceEventDTO>>> GetEvents([FromRoute] int organizationId)
    {
        var response = await _sender.Send(new GetAbsenceEventsQuery(organizationId));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(),
            accessDenied => Forbid()
        );
    }

    [HttpPost("events/{eventId}")]
    public async Task<ActionResult> Respond([FromRoute] int eventId, [FromQuery] bool accepted)
    {
        var response = await _sender.Send(new RespondAbsenceEventCommand(eventId, accepted));
        return response.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }
} 