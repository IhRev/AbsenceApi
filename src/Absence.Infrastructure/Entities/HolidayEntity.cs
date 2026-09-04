using Absence.Infrastructure.Common;

namespace Absence.Infrastructure.Entities;

public class HolidayEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Date { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
}