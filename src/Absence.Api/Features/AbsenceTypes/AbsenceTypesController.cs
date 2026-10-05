using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.AbsenceTypes;

[Authorize]
[ApiController]
[Route("organizations/{organizationId}/absences/types")]
public class AbsenceTypesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Get([FromRoute] int organizationId)
    {
        var result = await _sender.Send(new GetAbsenceTypes.Query(organizationId));
        return result.Match<ActionResult>(
            success => Ok(success.Value),
            notFound => NotFound()
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromRoute] int organizationId, [FromBody] CreateAbsenceTypeRequest request)
    {
        var result = await _sender.Send(new AddAbsenceType.Command(organizationId, request));
        return result.Match<ActionResult>(
            success => Ok(success.Value),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPut("{typeId}")]
    public async Task<ActionResult> Rename([FromRoute] int organizationId, [FromRoute] int typeId, [FromBody] RenameAbsenceTypeRequest request)
    {
        var result = await _sender.Send(new RenameAbsenceType.Command(organizationId, typeId, request));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpDelete("{typeId}")]
    public async Task<ActionResult> Delete([FromRoute] int organizationId, [FromRoute] int typeId)
    {
        var result = await _sender.Send(new DeleteAbsenceType.Command(organizationId, typeId));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }
}
