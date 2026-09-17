using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Users;

[Authorize]
[ApiController]
[Route("users")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet("details")]
    public async Task<ActionResult<UserDetails>> GetUserDetails()
    {
        var result = await sender.Send(new GetUserDetails.Query());
        return result.Match<ActionResult>(
            success => Ok(success.Value),
            unauthorized => Unauthorized()
        );
    }

    [HttpPut("details")]
    public async Task<ActionResult> UpdateUserDetails([FromBody] UpdateUserRequest userDetails)
    {
        var result = await sender.Send(new UpdateUser.Command(userDetails));
        return result.Match<ActionResult>(
            success => Ok(),
            unauthorized => Unauthorized()
        );
    }

    [HttpPut("change_password")]
    public async Task<ActionResult> UpdateUserPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await sender.Send(new ChangePassword.Command(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message),
            unauthorized => Unauthorized()
        );
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var result = await sender.Send(new DeleteUser.Command(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message),
            unauthorized => Unauthorized()
        );
    }
}
