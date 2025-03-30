using System.ComponentModel.DataAnnotations;

namespace Absence.Application.Common.DTOs;

internal class CreateAbsenceDTO
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = null!;
    [Required]
    public int Type { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}