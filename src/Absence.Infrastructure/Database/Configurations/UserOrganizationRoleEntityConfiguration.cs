using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class UserOrganizationRoleEntityConfiguration : EntityConfiguration<UserOrganizationRoleEntity, int>
{
    public override void Configure(EntityTypeBuilder<UserOrganizationRoleEntity> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(x => x.OrganizationRole)
            .WithMany(x => x.UserOrganizationRoles)
            .HasForeignKey(x => x.OrganizationRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.UserOrganizationRoles)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.ShortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Department)
            .WithMany(x => x.UserOrganizationRoles)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}