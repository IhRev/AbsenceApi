using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class UserOrganizationRoleEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public int OrganizationRoleId { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public OrganizationRoleEntity OrganizationRole { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public DepartmentEntity Department { get; set; } = null!;
}