using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationUserEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public bool IsAdmin { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}