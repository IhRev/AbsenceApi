namespace Absence.Application.UseCases.Absences.DTOs;

public class AbsenceEventDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int AbsenceType { get; set; }
    public required string User { get; set; }
    public int AbsenceEventType { get; set; }
} 