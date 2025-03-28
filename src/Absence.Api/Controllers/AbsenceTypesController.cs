﻿using Absence.Application.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("absence_types")]
public class AbsenceTypesController(ILogger<AbsenceTypesController> logger) : ControllerBase
{
    private readonly ILogger<AbsenceTypesController> _logger = logger;

    [HttpGet]
    public ActionResult<IEnumerable<AbsenceTypeDTO>> Get()
    {
        try
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
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
}