using Absence.Infrastructure.Common;

namespace Absence.Infrastructure.Entities;

public class AbsenceTypeEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; } 
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}