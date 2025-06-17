using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Holidays.DTOs;

public class CreateHolidayDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required()]
    public required DateTimeOffset Date { get; set; }
    [Required()]
    public required int OrganizationId { get; set; }
}