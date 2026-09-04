using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Database.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Absence.Api.Features.Holidays;

public static class GetHolidays
{
    public sealed class Query(int organizationId, DateTimeOffset startDate, DateTimeOffset endDate)
        : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
    {
        public int OrganizationId { get; } = organizationId;
        public DateTimeOffset StartDate { get; } = startDate;
        public DateTimeOffset EndDate { get; } = endDate;
    }

    internal sealed class Handler(
        AbsenceContext db,
        IUser user
    ) : IRequestHandler<Query, OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
    {
        public async Task<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>> Handle(Query request, CancellationToken cancellationToken)
        {
            var organizationUser = await db.OrganizationUsers.FirstOrDefaultAsync(
                _ => _.UserId == user.ShortId && _.OrganizationId == request.OrganizationId,
                cancellationToken);
            if (organizationUser is null)
            {
                return new BadRequest($"No organization with id {request.OrganizationId} found.");
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
