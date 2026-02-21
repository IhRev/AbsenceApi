using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class OrganizationRolePermissionEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public int OrganizationRoleId { get; set; }
    public int PermissionId { get; set; }
    public OrganizationRoleEntity OrganizationRole { get; set; } = null!;
    public PermissionEntity Permission { get; set; } = null!;
}