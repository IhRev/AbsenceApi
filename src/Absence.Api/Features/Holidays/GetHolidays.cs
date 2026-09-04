using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public static class GetHolidays
{
    public sealed class Query(int organizationId, DateTimeOffset startDate, DateTimeOffset endDate)
        : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, NotFound>>
    {
        public int OrganizationId { get; } = organizationId;
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IOrganizationAccess organizationAccess
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<HolidayDTO>>, NotFound>>
    {
        public async Task<OneOf<Success<IEnumerable<HolidayDTO>>, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var access = await organizationAccess.RequireMemberAsync(request.OrganizationId, cancellationToken);
            if (!access.TryPickT0(out _, out _))
            {
                return new NotFound();
            }

            var holidays = await db.Holidays
                .Where(_ => _.OrganizationId == request.OrganizationId)
                .Where(_ => _.Date >= request.StartDate && _.Date <= request.EndDate)
                .Select(_ => new HolidayDTO
                {
                    Id = _.Id,
                    Name = _.Name,
                    Date = _.Date
                })
                .ToListAsync(cancellationToken);
            return new Success<IEnumerable<HolidayDTO>>(holidays);
        }
    }
}
