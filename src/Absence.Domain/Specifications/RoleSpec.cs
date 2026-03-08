using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class RoleSpec : Specification<OrganizationRoleEntity>
{
    public RoleSpec(int organizationId, string name)
    {
        Query
            .Where(x => 
                x.OrganizationId == organizationId 
                && x.Name == name
            );
    }
}