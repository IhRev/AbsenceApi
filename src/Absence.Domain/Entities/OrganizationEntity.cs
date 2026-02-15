using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class OrganizationEntity : IIdKeyed<int>, ISoftDelete
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsDeleted { get; set; }
    public int OwnerId { get; set; }
    public UserEntity Owner { get; set; } = null!;
    public ICollection<OrganizationRoleEntity> OrganizationRoles { get; set; } = [];
    public ICollection<LeaveBalanceEntity> LeaveBalance { get; set; } = [];
    public ICollection<AbsenceEntity> Absences { get; set; } = [];
    public ICollection<AbsenceTypeEntity> AbsenceTypes { get; set; } = [];
    public ICollection<EventEntity> Events { get; set; } = [];
    public ICollection<DepartmentEntity> Departments { get; set; } = [];
    public ICollection<AbsenceRequestEntity> AbsenceRequests { get; set; } = [];
    public ICollection<OrganizationUserInvitationEntity> OrganizationUserInvitations { get; set; } = [];
}