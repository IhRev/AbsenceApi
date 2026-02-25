using Absence.Domain.Interfaces;

namespace Absence.Domain.Entities;

public class EventEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool NonWorkingDay { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
}