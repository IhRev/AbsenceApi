using Absence.Domain.Entities;
using Ardalis.Specification;

namespace Absence.Domain.Specifications;

public class EventsSpec : Specification<EventEntity>
{
    public EventsSpec(int organizationId, DateTime startDate, DateTime endDate)
    {
        Query
            .Where(_ => 
                _.OrganizationId == organizationId 
                && _.Date > startDate 
                && _.Date < endDate
            );
    }
}