using AbsenceApi.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsenceApi.Database.EntityConfigurations;

public class OrganizationEntityConfiguration : EntityConfiguration<OrganizationEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        base.Configure(builder);
    }
}