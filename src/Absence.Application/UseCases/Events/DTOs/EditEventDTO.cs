using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Events.DTOs;

public class EditEventDTO
{
    [Required]
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required]
    public bool NonWorkingDay { get; set; }
    [Required]
    public required DateTimeOffset Date { get; set; }
}