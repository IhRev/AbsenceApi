using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationUserEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public bool IsAdmin { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public virtual required OrganizationEntity Organization { get; set; }
    public UserEntity User { get; set; } = null!;
}