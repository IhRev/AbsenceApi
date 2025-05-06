using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Users.DTOs;

public class RegisterDTO
{
    [Required(AllowEmptyStrings = false)]
    public string FirstName { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    public string LastName { get; set; } = null!;
    [Required]
    public required UserCredentials Credentials { get; set; }
}