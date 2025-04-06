using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class UserCredentialsDTO
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Password { get; set; }
}