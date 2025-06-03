using Absence.Application.UseCases.AbsenceStatuses.DTOs;
using Absence.Application.UseCases.AbsenceStatuses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("absence_statuses")]
public class AbsenceStatusesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceStatusDTO>>> Get()
    {
        var statuses = await _sender.Send(new GetAllAbsenceStatusesQuery());
        return Ok(statuses);
    }
}