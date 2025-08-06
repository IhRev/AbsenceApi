using Absence.Application.Common.DTOs;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.DTOs;
using Absence.Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("details")]
    public async Task<ActionResult<UserDetails>> GetUserDetails()
    {
        var details = await _sender.Send(new GetUserDetailsQuery());
        return Ok(details);
    }

    [HttpPut("details")]
    public async Task<ActionResult> UpdateUserDetails([FromBody] UserDetails userDetails)
    {
        await _sender.Send(new UpdateUserCommand(userDetails));
        return Ok();
    }

    [HttpPut("change_password")]
    public async Task<ActionResult> UpdateUserPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _sender.Send(new ChangePasswordCommand(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var result = await _sender.Send(new DeleteUserCommand(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message)
        );
    }
}  