using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class HasPermissionSpec : Specification<UserOrganizationRoleEntity>
{
    public HasPermissionSpec(string permission, int organizationId, int userId, int? departmentId = null)
    {
        Query
            .Where(_ => 
                _.OrganizationId == organizationId 
                && _.UserId == userId
                && _.DepartmentId == departmentId
                && _.OrganizationRole.OrganizationRolePermissions.Any(orp => orp.Permission.Name == permission)
            );
    }
}