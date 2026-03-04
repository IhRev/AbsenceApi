using Absence.Application.UseCases.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("permissions")]
public class PermissionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int organizationId) 
        => Ok(await sender.Send(new GetAllPermissionsQuery(organizationId)));
}