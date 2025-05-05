using Absence.Application.Common.DTOs;
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
    public ActionResult<UserDetails> GetDetails()
    {
        return Ok(new UserDetails() { FirstName = "Ihor", LastName = "Reva" });
    }
}