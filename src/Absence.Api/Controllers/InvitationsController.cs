using Absence.Application.UseCases.Invitations.Commands;
using Absence.Application.UseCases.Invitations.DTOs;
using Absence.Application.UseCases.Invitations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("invitations")]
public class InvitationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvitationDTO>>> Get()
    {
        var invitations = await sender.Send(new GetUserInvitationsQuery());
        return Ok(invitations);
    }

    [HttpPost]
    public async Task<ActionResult> SendInvitation([FromBody] InviteUserToOrganizationDTO ivitation)
    {
        var response = await sender.Send(new InviteUserToOrganizationCommand(ivitation));
        return response.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPost("{initationId}")]
    public async Task<ActionResult> AcceptInvitation([FromRoute] int initationId, [FromQuery] bool accepted)
    {
        var response = await sender.Send(new AcceptInvitationCommand(initationId, accepted));
        return response.Match<ActionResult>(
            success => Ok(),
            badRequest => NotFound(),
            accessDenied => Forbid()
        );
    }
}