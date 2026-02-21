using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class PermissionEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<OrganizationRolePermissionEntity> OrganizationRolePermissions { get; set; } = [];
}