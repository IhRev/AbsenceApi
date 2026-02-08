using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class LeaveBalanceEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int TotalDays { get; set; }
    public int AvailableDays { get; set; }
    public int Year { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}