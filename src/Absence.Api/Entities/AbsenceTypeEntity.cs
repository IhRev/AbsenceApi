using AbsenceApi.Common;

namespace AbsenceApi.Entities;

public class AbsenceTypeEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}