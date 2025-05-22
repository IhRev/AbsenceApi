using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceTypeEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; } 
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}