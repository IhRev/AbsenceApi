using Absence.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Absence.Domain.Entities;

public class UserEntity : IdentityUser, IIdKeyed<string>
{
    public required string FirstName { get; set; }
    public required string SecondName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpireTimeInDays { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}