using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class DepartmentSpec : Specification<DepartmentEntity>
{
    public DepartmentSpec(int organizationId)
    {
        Query.Where(_ => _.OrganizationId == organizationId);
    }
}