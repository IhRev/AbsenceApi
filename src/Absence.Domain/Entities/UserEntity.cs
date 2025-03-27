using Absence.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Absence.Domain.Entities;

public class UserEntity : IdentityUser, IIdKeyed<string>
{
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = null!;
}