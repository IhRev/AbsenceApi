using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationUserInvitationEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public virtual OrganizationEntity Organization { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}