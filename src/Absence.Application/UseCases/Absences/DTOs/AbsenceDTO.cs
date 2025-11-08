namespace Absence.Application.UseCases.Absences.DTOs;

public class AbsenceDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int Type { get; set; }
    public int UserId { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
}