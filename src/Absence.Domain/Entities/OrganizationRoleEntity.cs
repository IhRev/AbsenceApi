using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;
 
public class OrganizationRoleEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Level { get; set; }
    public bool IsSystemRole { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<UserOrganizationRoleEntity> UserOrganizationRoles { get; set; } = [];
    public ICollection<OrganizationRolePermissionEntity> OrganizationRolePermissions { get; set; } = [];
}