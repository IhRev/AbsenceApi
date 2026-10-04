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
}
