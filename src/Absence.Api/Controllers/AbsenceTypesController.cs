using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("absences/types")]
public class AbsenceTypesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Get([FromQuery] int organizationId)
    {
        var response = await sender.Send(new GetAllAbsenceTypesQuery(organizationId));
        return response.Match<ActionResult>( 
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Add([FromQuery] int organizationId, [FromBody] CreateAbsenceTypeDTO absenceType)
    {
        var response = await sender.Send(new CreateAbsenceTypeCommand(organizationId, absenceType));
        return response.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    }
}