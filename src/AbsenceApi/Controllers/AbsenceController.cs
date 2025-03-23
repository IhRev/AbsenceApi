using Microsoft.AspNetCore.Mvc;

namespace AbsenceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AbsenceController(ILogger<AbsenceController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        return Ok();
    }
}