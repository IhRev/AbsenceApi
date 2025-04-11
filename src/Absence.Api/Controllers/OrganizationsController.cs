﻿using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Organizations.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] CreateOrganizationDTO organization)
    {
        var id = await _sender.Send(new AddOrganizationCommand(organization));
        return Ok(id);
    }
}