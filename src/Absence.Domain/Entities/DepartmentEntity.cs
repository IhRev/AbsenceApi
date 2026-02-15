using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class DepartmentEntity : IIdKeyed<int>, ISoftDelete
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsDeleted { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<DepartmentUserEntity> DepartmentUsers { get; set; } = [];
    public ICollection<UserOrganizationRoleEntity> UserOrganizationRoles { get; set; } = [];
}