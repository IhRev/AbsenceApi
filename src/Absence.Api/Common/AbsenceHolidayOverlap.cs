namespace Absence.Api.Common;

internal static class AbsenceHolidayOverlap
{
    // Both ends count. A same-day absence is one instant, so an exclusive end would miss a holiday at that instant.
    public static bool Contains(DateTimeOffset start, DateTimeOffset end, DateTimeOffset holiday) =>
        holiday >= start && holiday <= end;
}
