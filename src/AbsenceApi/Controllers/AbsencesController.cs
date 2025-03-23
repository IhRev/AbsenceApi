using AbsenceApi.DTOs;
using AbsenceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceApi.Controllers;

[ApiController]
[Route("absences")]
public class AbsencesController(ILogger<AbsencesController> logger, AbsenceMockService absenceService) : ControllerBase
{
    private static int lastId = 0;
    private readonly AbsenceMockService _absenceService = absenceService;

    [HttpGet]
    public ActionResult<IEnumerable<AbsenceDTO>> Get()
    {
        try
        {
            return Ok(_absenceService.GetAll());
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<int> Add([FromBody] AbsenceDTO absence)
    {
        try
        {
            return Ok(_absenceService.Add(absence));
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
} 