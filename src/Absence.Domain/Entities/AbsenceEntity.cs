using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int AbsenceTypeId { get; set; }
    public string UserId { get; set; } = null!;
    public AbsenceTypeEntity AbsenceType { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}