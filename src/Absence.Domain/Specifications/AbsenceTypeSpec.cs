using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class AbsenceTypeSpec : Specification<AbsenceTypeEntity>
{
    public AbsenceTypeSpec(int organizationId)
    {
        Query.Where(_ => _.OrganizationId == organizationId);
    }
}