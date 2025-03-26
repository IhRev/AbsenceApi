using AbsenceApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceApi.Controllers;

public class UsersController(ILogger<AbsencesController> logger) : ControllerBase
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
}