using AbsenceApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceApi.Controllers;

[ApiController]
[Route("absence_types")]
public class AbsenceTypeController(ILogger<AbsenceTypeController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<AbsenceTypeDTO>> Get()
    {
        return Ok(
            new[] 
            { 
                new AbsenceTypeDTO() 
                { 
                    Id = 1, 
                    Name = "Remote Work" 
                }, 
                new AbsenceTypeDTO()
                {
                    Id = 2,
                    Name = "Vacation Leave"
                },
                new AbsenceTypeDTO()
                {
                    Id = 3,
                    Name = "Sick"
                }
            } 
        );
    }
}