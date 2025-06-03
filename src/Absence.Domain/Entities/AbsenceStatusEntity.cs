using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceStatusEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public AbsenceStatus Name { get; set; }
    public ICollection<AbsenceEntity> Absences { get; set; } = [];
}