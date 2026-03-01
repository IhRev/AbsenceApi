namespace Absence.Application.UseCases.AbsenceTypes.DTOs;

public class AbsenceTypeDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public bool RequiresApproval { get; set; }
    public bool CountsTowardAnnualLeave { get; set; }
}