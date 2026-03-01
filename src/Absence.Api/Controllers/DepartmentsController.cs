using Absence.Application.UseCases.Departments.Commands;
using Absence.Application.UseCases.Departments.DTOs;
using Absence.Application.UseCases.Departments.Queries;
using Absence.Application.UseCases.Events.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("organizations/{organizationId}/departments")]
public class DepartmentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDTO>>> Get([FromRoute] int organizationId) 
        => Ok(await sender.Send(new GetAllDepartmentsQuery(organizationId)));

    [HttpPost]
    public async Task<ActionResult<int>> Add(
        [FromRoute] int organizationId,
        [FromBody] CreateDepartmentDTO department
    ) => Ok(await sender.Send(new AddDepartmentCommand(organizationId, department)));

    [HttpPut]
    public async Task<ActionResult> Edit([FromRoute] int organizationId, [FromBody] EditDepartmentDTO department)
    {
        var result = await sender.Send(new EditDepartmentCommand(organizationId, department));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int organizationId, [FromRoute] int id)
    {
        var result = await sender.Send(new DeleteDepartmentCommand(organizationId, id));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }
}