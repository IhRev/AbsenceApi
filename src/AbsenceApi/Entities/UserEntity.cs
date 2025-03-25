using AbsenceApi.Common;
using Microsoft.AspNetCore.Identity;

namespace AbsenceApi.Entities;

public class UserEntity : IdentityUser, IIdKeyed<string>
{
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public int OrganizationId { get; set; }
    public OrganizationEntity Organization { get; set; } = null!;
}