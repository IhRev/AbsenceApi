using Absence.Domain.Entities;
using Absence.Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class PermissionEntityConfiguration : EntityConfiguration<PermissionEntity, int>
{
    public override void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(p => p.Name)
            .IsRequired();
    }
}