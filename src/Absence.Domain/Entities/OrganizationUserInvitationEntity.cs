using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationUserInvitationEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int Inviter { get; set; }
    public int Invited { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public UserEntity InvitedUser { get; set; } = null!;
    public UserEntity InviterUser { get; set; } = null!;
}