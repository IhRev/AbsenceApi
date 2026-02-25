using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class AbsenceEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int AbsenceTypeId { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public int ApprovedBy { get; set; }
    public AbsenceTypeEntity AbsenceType { get; set; } = null!;
    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public UserEntity ApprovedByUser { get; set; } = null!;
}