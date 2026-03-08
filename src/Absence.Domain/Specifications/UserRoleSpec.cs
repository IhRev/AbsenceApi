using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class UserRoleSpec : Specification<UserOrganizationRoleEntity>
{
    public UserRoleSpec(int organizationId, int userId)
    {
        Query
            .Where(_ => 
                _.OrganizationId == organizationId 
                && _.UserId == userId
            );
    }

    public UserRoleSpec(string permission, int organizationId, int userId)
    {
        Query
            .Where(_ =>
                _.OrganizationId == organizationId
                && _.UserId == userId
                && _.OrganizationRole.OrganizationRolePermissions.Any(orp => orp.Permission.Name == permission)
            );
    }

    public UserRoleSpec(string permission, int organizationId, int userId, int departmentId)
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