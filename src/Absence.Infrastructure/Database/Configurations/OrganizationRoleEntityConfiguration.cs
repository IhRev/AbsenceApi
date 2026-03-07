using Absence.Domain.Entities;
using Absence.Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class OrganizationRoleEntityConfiguration : EntityConfiguration<OrganizationRoleEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationRoleEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder
            .Property(x => x.Level)
            .IsRequired();

        builder
            .HasOne(x => x.Organization)
            .WithMany(x => x.OrganizationRoles)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}