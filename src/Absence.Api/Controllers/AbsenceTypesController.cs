using Absence.Application.UseCases.AbsenceTypes.Commands;
using Absence.Application.UseCases.AbsenceTypes.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("organizations/{organizationId}/absences/types")]
public class AbsenceTypesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Get([FromQuery] int organizationId) 
        => Ok(await sender.Send(new GetAllAbsenceTypesQuery(organizationId)));

    [HttpPost]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Add(
        [FromQuery] int organizationId,
        [FromBody] CreateAbsenceTypeDTO absenceType
    ) => Ok(await sender.Send(new CreateAbsenceTypeCommand(organizationId, absenceType)));

    [HttpPut]
    public async Task<ActionResult> Edit([FromQuery] int organizationId, [FromBody] UpdateAbsenceTypeDTO absenceType)
    {
        var response = await sender.Send(new UpdateAbsenceTypeCommand(organizationId, absenceType));
        return response.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromQuery] int organizationId, [FromRoute] int id)
    {
        var response = await sender.Send(new DeleteAbsenceTypeCommand(organizationId, id));
        return response.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }
}