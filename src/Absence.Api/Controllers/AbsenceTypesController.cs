using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.AbsenceTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("absence_types")]
public class AbsenceTypesController(
    ILogger<AbsenceTypesController> logger,
    ISender sender
) : ControllerBase
{
    private readonly ILogger<AbsenceTypesController> _logger = logger;
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AbsenceTypeDTO>>> Get()
    {
        try
        {
            var types = await _sender.Send(new GetAllAbsenceTypesQuery());
            return Ok(types);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
}