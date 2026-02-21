using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class OrganizationRolePermissionEntityConfiguration : EntityConfiguration<OrganizationRolePermissionEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationRolePermissionEntity> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(x => x.OrganizationRole)
            .WithMany(x => x.OrganizationRolePermissions)
            .HasForeignKey(x => x.OrganizationRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Permission)
            .WithMany(x => x.OrganizationRolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}