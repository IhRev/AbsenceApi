using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class DepartmentSpec : Specification<DepartmentEntity>
{
    public DepartmentSpec(int organizationId, int userId)
    {
        Query
            .Include(_ => _.DepartmentUsers)
            .Where(_ => _.OrganizationId == organizationId && _.DepartmentUsers.Any(_ => _.UserId == userId));
    }
}