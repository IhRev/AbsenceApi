using Absence.Domain.Common;

namespace Absence.Domain.Entities;

public class OrganizationEntity : IIdKeyed<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OwnerId { get; set; }
    public UserEntity Owner { get; set; } = null!;
    public ICollection<AbsenceEntity> Absences { get; set; } = [];
    public ICollection<HolidayEntity> Holidays { get; set; } = [];
    public ICollection<OrganizationUserEntity> OrganizationsUsers { get; set; } = [];
    public ICollection<AbsenceEventEntity> AbsenceEvents { get; set; } = [];
    public ICollection<OrganizationUserInvitationEntity> OrganizationUserInvitations { get; set; } = [];
}