using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class DepartmentEntity : IIdKeyed<int>, ISoftDelete
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsDeleted { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<DepartmentUserEnitty> DepartmentUsers { get; set; } = [];
    public ICollection<UserOrganizationRoleEntity> UserOrganizationRoles { get; set; } = [];
}