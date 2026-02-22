namespace Absence.Application.UseCases.Events.DTOs;

public class EventDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool NonWorkingDay { get; set; }
    public required DateTimeOffset Date { get; set; }
}