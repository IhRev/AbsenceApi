namespace Absence.Api.Common;

internal static class AbsenceDateOrder
{
    public const string Message = "Start date must be before end date.";

    public static bool IsValid(DateTimeOffset start, DateTimeOffset end) => start <= end;
}
