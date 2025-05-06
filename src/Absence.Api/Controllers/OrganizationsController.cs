using Absence.Application.UseCases.Organizations.Commands;
using Absence.Application.UseCases.Organizations.DTOs;
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

    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] CreateOrganizationDTO organization)
    {
        var id = await _sender.Send(new AddOrganizationCommand(organization));
        return Ok(id);
    }
}