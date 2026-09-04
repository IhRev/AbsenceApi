using Absence.Api.Common.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Absence.Api.Common.Services;

internal class AbsenceHolidayOverlapChecker(AbsenceContext db) : IAbsenceHolidayOverlapChecker
{
    public Task<bool> AbsenceOverlapsHolidayAsync(
        int organizationId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default) =>
        db.Holidays.AnyAsync(
            _ => _.OrganizationId == organizationId && _.Date >= startDate && _.Date <= endDate,
            cancellationToken);

    public Task<bool> HolidayOverlapsAbsenceAsync(
        int organizationId,
        DateTimeOffset date,
        CancellationToken cancellationToken = default) =>
        db.Absences.AnyAsync(
            _ => _.OrganizationId == organizationId && _.StartDate <= date && _.EndDate >= date,
            cancellationToken);
}
