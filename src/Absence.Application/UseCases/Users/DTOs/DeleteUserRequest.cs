using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Users.DTOs;

public class DeleteUserRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}