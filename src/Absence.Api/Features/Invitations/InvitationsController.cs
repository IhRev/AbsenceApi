using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Invitations;

[Authorize]
[ApiController]
[Route("invitations")]
public class InvitationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvitationDTO>>> Get()
    {
        var invitations = await sender.Send(new GetUserInvitations.Query());
        return Ok(invitations);
    }

    [HttpPost]
    public async Task<ActionResult> SendInvitation([FromBody] InviteUserToOrganizationDTO invitation)
    {
        var response = await sender.Send(new InviteUser.Command(invitation));
        return response.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPost("{invitationId}")]
    public async Task<ActionResult> AcceptInvitation([FromRoute] int invitationId, [FromQuery] bool accepted)
    {
        var response = await sender.Send(new AcceptInvitation.Command(invitationId, accepted));
        return response.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }
}
