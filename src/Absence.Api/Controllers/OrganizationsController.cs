﻿using Absence.Application.UseCases.Organizations.Commands;
using Absence.Application.UseCases.Organizations.DTOs;
using Absence.Application.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("organizations")]
public class OrganizationsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> Get()
    {
        var organizations = await _sender.Send(new GetUserOrganizationsQuery());
        return Ok(organizations);
    }

    [HttpGet("{organizationId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> Get([FromRoute] int organizationId)
    {
        var result = await _sender.Send(new GetOrganizationMembersQuery(organizationId));
        return result.Match<ActionResult>(
            success => Ok(success.Value),    
            badRequest => Ok(badRequest.Message),    
            accessDenied => Forbid()    
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateOrganizationDTO organization)
    {
        var id = await _sender.Send(new AddOrganizationCommand(organization));
        return Ok(id);
    }
}