using Absence.Application.Common.Interfaces;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;

namespace Absence.Application.Common.Services;

internal class AbsenceHolidayOverlapChecker(
    IRepository<HolidayEntity> holidayRepository,
    IRepository<AbsenceEntity> absenceRepository
) : IAbsenceHolidayOverlapChecker
{
    private readonly IRepository<HolidayEntity> _holidayRepository = holidayRepository;
    private readonly IRepository<AbsenceEntity> _absenceRepository = absenceRepository;

    public async Task<bool> AbsenceOverlapsHolidayAsync(
        int organizationId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var holiday = await _holidayRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.OrganizationId == organizationId),
                q => q.Where(_ => _.Date >= startDate && _.Date <= endDate)
            ],
            cancellationToken
        );

        return holiday is not null;
    }

    public async Task<bool> HolidayOverlapsAbsenceAsync(
        int organizationId,
        DateTimeOffset date,
        CancellationToken cancellationToken = default)
    {
        var absence = await _absenceRepository.GetFirstOrDefaultAsync(
            [
                q => q.Where(_ => _.OrganizationId == organizationId),
                q => q.Where(_ => _.StartDate <= date && _.EndDate >= date)
            ],
            cancellationToken
        );

        return absence is not null;
    }
}