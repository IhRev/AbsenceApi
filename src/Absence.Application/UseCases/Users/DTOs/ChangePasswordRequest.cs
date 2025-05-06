using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Users.DTOs;

public class ChangePasswordRequest
{
    [Required(AllowEmptyStrings = false)]
    public required string OldPassword { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string NewPassword { get; set; }
}