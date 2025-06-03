using Absence.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Contexts;

public class AbsenceContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{
    public virtual DbSet<AbsenceEntity> Absences { get; set; }
    public virtual DbSet<AbsenceTypeEntity> AbsenceTypes { get; set; }
    public virtual DbSet<HolidayEntity> Holidays { get; set; }
    public virtual DbSet<OrganizationEntity> Organizations { get; set; }
    public virtual DbSet<OrganizationUserEntity> OrganizationUsers { get; set; }
    public virtual DbSet<OrganizationUserInvitationEntity> OrganizationUserInvitations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AbsenceContext).Assembly);
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
    }
}