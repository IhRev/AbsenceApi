using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

public class CreateAbsenceDTO
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = null!;
    [Required]
    public int Type { get; set; }
    [Required]
    public DateTimeOffset StartDate { get; set; }
    [Required]
    public DateTimeOffset EndDate { get; set; }
}