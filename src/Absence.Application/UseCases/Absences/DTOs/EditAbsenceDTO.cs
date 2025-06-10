using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Absences.DTOs;

public class EditAbsenceDTO
{
    [Required]
    public required int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public required int Type { get; set; }
    [Required]
    public required DateTimeOffset StartDate { get; set; }
    [Required]
    public required DateTimeOffset EndDate { get; set; }
}