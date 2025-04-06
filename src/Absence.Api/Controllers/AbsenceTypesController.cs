using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("absence_types")]
public class AbsenceTypesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Get()
    {
        var types = await _sender.Send(new GetAllAbsenceTypesQuery());
        return Ok(types);
    }
}