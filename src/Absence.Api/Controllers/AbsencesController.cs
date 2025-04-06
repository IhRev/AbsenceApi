using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Absences.Commands;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceDTO>>> Get()
    {
        var absences = await _sender.Send(new GetUserAbsencesQuery("id"));
        return Ok(absences);
    } 

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateAbsenceDTO absence)
    {
        int id = await _sender.Send(new AddAbsenceCommand(absence, "id"));
        return Ok(id);
    }
} 