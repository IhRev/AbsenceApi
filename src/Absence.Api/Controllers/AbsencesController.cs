using Absence.Application.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("absences")]
public class AbsencesController(ILogger<AbsencesController> logger) : ControllerBase
{
    private readonly ILogger<AbsencesController> _logger = logger;

    [HttpGet]
    public ActionResult<IEnumerable<AbsenceDTO>> Get()
    {
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<int> Add([FromBody] AbsenceDTO absence)
    {
        try
        {
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
} 