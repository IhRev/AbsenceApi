using Absence.Domain.Common;
using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class AbsenceRequestEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public AbsenceEventType RequestType { get; set; }
    public int? OldAbsenceTypeId { get; set; }
    public int? NewAbsenceTypeId { get; set; }
    public DateTimeOffset? OldStartDate { get; set; }
    public DateTimeOffset? NewStartDate { get; set; }
    public DateTimeOffset? OldEndDate { get; set; }
    public DateTimeOffset? NewEndDate { get; set; }
    public string? OldName { get; set; }
    public string? NewName { get; set; }
    public int? AbsenceId { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}