namespace Absence.Application.UseCases.AbsenceTypes.DTOs;

public class UpdateAbsenceTypeDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required bool RequiresApproval { get; set; }
    public required bool CountsTowardAnnualLeave { get; set; }
}