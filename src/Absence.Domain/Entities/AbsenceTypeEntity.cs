using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceTypeEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}