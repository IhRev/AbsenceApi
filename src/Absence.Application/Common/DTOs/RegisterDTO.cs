using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class RegisterDTO
{
    [Required(AllowEmptyStrings = false)]
    public string FirstName { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    public string SecondName { get; set; } = null!;
    [Required]
    public required UserCredentialsDTO Credentials { get; set; }
}