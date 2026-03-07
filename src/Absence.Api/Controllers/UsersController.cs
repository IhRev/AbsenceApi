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
    [HttpGet("details")]
    public async Task<ActionResult<UserDetails>> GetUserDetails()
    {
        var result = await sender.Send(new GetUserDetailsQuery());
        return result.Match<ActionResult<UserDetails>>(
            userDetails => Ok(userDetails),
            notFound => NotFound()
        );
    }

    [HttpPut("details")]
    public async Task<ActionResult> UpdateUserDetails([FromBody] UserDetails userDetails)
    {
        var result = await sender.Send(new UpdateUserCommand(userDetails));
        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound()
        );
    }

    [HttpPut("change_password")]
    public async Task<ActionResult> UpdateUserPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await sender.Send(new ChangePasswordCommand(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message),
            notFound => NotFound()
        );
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var result = await sender.Send(new DeleteUserCommand(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message),
            notFound => NotFound()
        );
    }
}  