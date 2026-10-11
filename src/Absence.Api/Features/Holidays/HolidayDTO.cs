namespace Absence.Api.Features.Holidays;

public class HolidayDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Date { get; set; }
}
