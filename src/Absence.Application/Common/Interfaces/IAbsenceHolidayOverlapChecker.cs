namespace Absence.Application.Common.Interfaces;

public interface IAbsenceHolidayOverlapChecker
{
    Task<bool> AbsenceOverlapsHolidayAsync(
        int organizationId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    Task<bool> HolidayOverlapsAbsenceAsync(
        int organizationId,
        DateTimeOffset date,
        CancellationToken cancellationToken = default);
}