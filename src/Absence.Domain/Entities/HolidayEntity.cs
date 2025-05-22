using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class HolidayEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
}