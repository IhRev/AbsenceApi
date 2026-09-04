using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Absence.Api.Features.Users;

[Authorize]
[ApiController]
[Route("users")]
public class UsersController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("details")]
    public async Task<ActionResult<UserDetails>> GetUserDetails()
    {
        var details = await _sender.Send(new GetUserDetails.Query());
        return Ok(details);
    }

    [HttpPut("details")]
    public async Task<ActionResult> UpdateUserDetails([FromBody] UserDetails userDetails)
    {
        await _sender.Send(new UpdateUser.Command(userDetails));
        return Ok();
    }

    [HttpPut("change_password")]
    public async Task<ActionResult> UpdateUserPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _sender.Send(new ChangePassword.Command(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message)
        );
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var result = await _sender.Send(new DeleteUser.Command(request));
        return result.Match<ActionResult>(
            success => Ok(),
            badRequest => BadRequest(badRequest.Message)
        );
    }
}
