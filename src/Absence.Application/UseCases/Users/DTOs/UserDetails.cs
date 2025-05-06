using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class UserDetails
{
    [Required(AllowEmptyStrings = false)]
    public required string FirstName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string LastName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Email { get; set; }
}