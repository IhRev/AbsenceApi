using Absence.Application.Common.Interfaces;

namespace Absence.Infrastructure.Time;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => 
        DateTimeOffset.UtcNow;
}