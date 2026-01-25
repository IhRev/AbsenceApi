using Absence.Application.UseCases.Organizations.Commands;
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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> Get()
    {
        var organizations = await sender.Send(new GetUserOrganizationsQuery());
        return Ok(organizations);
    }

    [HttpGet("{organizationId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> Get([FromRoute] int organizationId)
    {
        var result = await sender.Send(new GetOrganizationMembersQuery(organizationId));
        return result.Match<ActionResult>(
            success => Ok(success.Value),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpPost]
    public async Task<ActionResult<int>> Add([FromBody] CreateOrganizationDTO organization)
    {
        var id = await sender.Send(new AddOrganizationCommand(organization));
        return Ok(id);
    }

    [HttpDelete("{organizationId}")]
    public async Task<ActionResult> Delete([FromRoute] int organizationId, [FromBody] DeleteOrganizationRequest request)
    {
        var result = await sender.Send(new DeleteOrganizationCommand(organizationId, request));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            accessDenied => Forbid()
        );
    }

    [HttpPut("{organizationId}/members/{memberId}")]
    public async Task<ActionResult> ChangeAccess([FromRoute] int organizationId, [FromRoute] int memberId , [FromQuery] bool isAdmin)
    {
        var result = await sender.Send(new ChangeMemberAccessCommand(organizationId, memberId, isAdmin));
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
        var result = await sender.Send(new DeleteMemberCommand(organizationId, memberId));
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
        var result = await sender.Send(new EditOrganizationCommand(editOrganizationDTO));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            badRequest => BadRequest(badRequest.Message),
            accessDenied => Forbid()
        );
    }
}