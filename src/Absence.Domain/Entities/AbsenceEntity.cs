using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class AbsenceEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
    public required int AbsenceTypeId { get; set; }
    public required int UserId { get; set; }
    public int OrganizationId { get; set; }
    public AbsenceTypeEntity AbsenceType { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public OrganizationEntity Organization { get; set; } = null!;
}