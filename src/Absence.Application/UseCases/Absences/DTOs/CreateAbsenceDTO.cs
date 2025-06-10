using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Absences.DTOs;

public class CreateAbsenceDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public int Type { get; set; }
    [Required]
    public DateTimeOffset StartDate { get; set; }
    [Required]
    public DateTimeOffset EndDate { get; set; }
    [Required]
    public int Organization { get; set; }
}