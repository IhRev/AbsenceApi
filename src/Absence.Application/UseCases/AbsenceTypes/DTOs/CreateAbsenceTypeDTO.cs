namespace Absence.Application.UseCases.AbsenceTypes.DTOs;

public class CreateAbsenceTypeDTO
{
    public required string Name { get; set; }
    public bool CountsTowardAnnualLeave { get; set; }
}