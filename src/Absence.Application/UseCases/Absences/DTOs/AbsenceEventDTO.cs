namespace Absence.Application.UseCases.Absences.DTOs;

public class AbsenceEventDTO
{
    public int Id { get; set; }
    public int? AbsenceId { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? AbsenceType { get; set; }
    public int UserId { get; set; }
    public int AbsenceEventType { get; set; }
} 