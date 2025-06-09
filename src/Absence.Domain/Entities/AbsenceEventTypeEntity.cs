using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceEventTypeEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public AbsenceEventType Name { get; set; }
    public ICollection<AbsenceEventEntity> AbsenceEvents { get; set; } = [];
}