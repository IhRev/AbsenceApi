using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class AbsenceTypeEntity : IIdKeyed<int>, ISoftDelete
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public bool RequiresApproval { get; set; }
    public bool CountsTowardAnnualLeave { get; set; }
    public bool IsDeleted { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}