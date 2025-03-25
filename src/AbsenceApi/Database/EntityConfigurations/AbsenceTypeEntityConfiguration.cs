using AbsenceApi.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsenceApi.Database.EntityConfigurations;

public class AbsenceTypeEntityConfiguration : EntityConfiguration<AbsenceEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceEntity> builder)
    {
        base.Configure(builder);
    }
}