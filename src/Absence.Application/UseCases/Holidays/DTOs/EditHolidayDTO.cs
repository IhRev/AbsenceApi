namespace Absence.Application.UseCases.Holidays.DTOs;

public class EditHolidayDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
}