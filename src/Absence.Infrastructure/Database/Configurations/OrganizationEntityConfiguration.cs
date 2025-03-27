using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class OrganizationEntityConfiguration : EntityConfiguration<OrganizationEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(_ => _.Name)
            .IsRequired();
    }
}