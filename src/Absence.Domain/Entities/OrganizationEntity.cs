using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<UserEntity> Members { get; set; } = null!;
}