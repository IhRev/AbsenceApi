namespace Absence.Application.UseCases.Holidays.DTOs;

public class CreateHolidayDTO
{
    public required string Name { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
}