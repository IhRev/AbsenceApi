namespace Absence.Application.UseCases.Holidays.DTOs;

public class HolidayDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Date { get; set; }
}