using AbsenceApi.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsenceApi.Database.EntityConfigurations;

public class AbsenceTypeEntityConfiguration : EntityConfiguration<AbsenceTypeEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceTypeEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .IsRequired();
    }
}