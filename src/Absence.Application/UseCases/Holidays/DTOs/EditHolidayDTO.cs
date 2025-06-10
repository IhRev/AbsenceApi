using System.ComponentModel.DataAnnotations;

namespace Absence.Application.UseCases.Holidays.DTOs;

public class EditHolidayDTO
{
    [Required()]
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
    [Required()]
    public required DateTimeOffset StartDate { get; set; }
    [Required()]
    public required DateTimeOffset EndDate { get; set; }
}