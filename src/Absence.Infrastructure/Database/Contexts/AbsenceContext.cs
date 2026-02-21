using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Absence.Infrastructure.Database.Contexts;

public class AbsenceContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{
    public virtual DbSet<OrganizationEntity> Organizations { get; set; }
    public virtual DbSet<OrganizationRoleEntity> OrganizationRoles { get; set; }
    public virtual DbSet<UserOrganizationRoleEntity> UserOrganizationRoles { get; set; }
    public virtual DbSet<PermissionEntity> Permissions { get; set; }
    public virtual DbSet<OrganizationRolePermissionEntity> OrganizationRolePermissions { get; set; }
    public virtual DbSet<OrganizationUserInvitationEntity> OrganizationUserInvitations { get; set; }
    public virtual DbSet<DepartmentEntity> Departments { get; set; }
    public virtual DbSet<DepartmentUserEntity> DepartmentUsers { get; set; }
    public virtual DbSet<EventEntity> Events { get; set; }
    public virtual DbSet<LeaveBalanceEntity> LeaveBalance { get; set; }
    public virtual DbSet<AbsenceTypeEntity> AbsenceTypes { get; set; }
    public virtual DbSet<AbsenceEntity> Absences { get; set; }
    public virtual DbSet<AbsenceRequestEntity> AbsenceRequests { get; set; }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();

        foreach (var item in ChangeTracker.Entries<ISoftDelete>().Where(e => e.State == EntityState.Deleted))
        {
            item.State = EntityState.Modified;
            item.Entity.IsDeleted = true;
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

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