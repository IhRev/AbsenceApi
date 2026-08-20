using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceEventEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public int? AbsenceId { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int AbsenceTypeId { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public AbsenceEventType AbsenceEventType { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}