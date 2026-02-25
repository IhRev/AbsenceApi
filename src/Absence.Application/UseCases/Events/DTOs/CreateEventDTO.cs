using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Events.DTOs;

public class CreateEventDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public required DateTimeOffset Date { get; set; }
    [Required]
    public bool NonWorkingDay { get; set; }
}