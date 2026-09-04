using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Organizations;

[Authorize]
[ApiController]
[Route("organizations")]
public class OrganizationsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> Get()
    {
        var organizations = await _sender.Send(new GetUserOrganizations.Query());
        return Ok(organizations);
    }

    [HttpGet("{organizationId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> Get([FromRoute] int organizationId)
    {
        var result = await _sender.Send(new GetOrganizationMembers.Query(organizationId));
        return result.Match<ActionResult>(
            success => Ok(success.Value),
            notFound => NotFound()
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateOrganizationDTO organization)
    {
        var id = await _sender.Send(new AddOrganization.Command(organization));
        return Ok(id);
    }

    [HttpDelete("{organizationId}")]
    public async Task<ActionResult> Delete([FromRoute] int organizationId, [FromBody] DeleteOrganizationRequest request)
    {
        var result = await _sender.Send(new DeleteOrganization.Command(organizationId, request));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpPut("{organizationId}/members/{memberId}")]
    public async Task<ActionResult> ChangeAccess([FromRoute] int organizationId, [FromRoute] int memberId , [FromQuery] bool isAdmin)
    {
        var result = await _sender.Send(new ChangeMemberAccess.Command(organizationId, memberId, isAdmin));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid(),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpDelete("{organizationId}/members/{memberId}")]
    public async Task<ActionResult> DeleteMember([FromRoute] int organizationId, [FromRoute] int memberId)
    {
        var result = await _sender.Send(new global::Absence.Api.Features.Organizations.DeleteMember.Command(organizationId, memberId));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }

    [HttpPut]
    public async Task<ActionResult> Edit([FromBody] EditOrganizationDTO editOrganizationDTO)
    {
        var result = await _sender.Send(new EditOrganization.Command(editOrganizationDTO));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }
}
