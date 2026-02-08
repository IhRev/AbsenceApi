using Absence.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Absence.Domain.Entities;

public class UserEntity : IdentityUser, IIdKeyed<string>, ISoftDelete
{
    public int ShortId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<LeaveBalanceEntity> LeaveBalance { get; set; } = [];
    public ICollection<UserOrganizationRoleEntity> UserOrganizationRoles { get; set; } = [];
    public ICollection<AbsenceEntity> Absences { get; set; } = [];
    public ICollection<AbsenceEntity> ApprovedAbsences { get; set; } = [];
    public ICollection<AbsenceRequestEntity> AbsenceRequests { get; set; } = [];
    public ICollection<OrganizationEntity> OwnedOrganizations { get; set; } = [];
    public ICollection<DepartmentUserEnitty> DepartmentUsers { get; set; } = [];
    public ICollection<OrganizationUserInvitationEntity> InvitationsSent { get; set; } = [];
    public ICollection<OrganizationUserInvitationEntity> InvitationsReceived { get; set; } = [];
}