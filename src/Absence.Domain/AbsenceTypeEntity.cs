namespace Absence.Domain;

public class AbsenceTypeEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}
