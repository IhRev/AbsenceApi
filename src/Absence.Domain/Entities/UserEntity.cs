using Absence.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Absence.Domain.Entities;

public class UserEntity : IdentityUser, IIdKeyed<string>
{
    public int ShortId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
    public ICollection<AbsenceEntity> Absences { get; set; } = [];
    public ICollection<OrganizationEntity> Organizations { get; set; } = [];
    public ICollection<OrganizationUserEntity> OrganizationsUsers { get; set; } = [];
    public ICollection<OrganizationUserInvitationEntity> OrganizationUserInvitations { get; set; } = [];
}