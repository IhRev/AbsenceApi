using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class CreateUserDTO
{
    [Required(AllowEmptyStrings = false)]
    public string FirstName { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    public string SecondName { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public int Organization { get; set; }
}