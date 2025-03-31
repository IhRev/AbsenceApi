using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Organizations.Commands;
using Absence.Application.UseCases.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationsController(
    ILogger<OrganizationsController> logger,
    ISender sender
) : ControllerBase
{
    private readonly ILogger<OrganizationsController> _logger = logger;
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] CreateOrganizationDTO organization)
    {
        try
        {
            var id = await _sender.Send(new AddOrganizationCommand(organization));
            return Ok(id);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "");
            return StatusCode(500);
        }
    }
}