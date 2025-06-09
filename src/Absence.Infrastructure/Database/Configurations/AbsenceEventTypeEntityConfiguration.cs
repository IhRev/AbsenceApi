using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

internal class AbsenceEventTypeEntityConfiguration : EntityConfiguration<AbsenceEventTypeEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceEventTypeEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .HasConversion<string>()
           .IsRequired();
    }
}