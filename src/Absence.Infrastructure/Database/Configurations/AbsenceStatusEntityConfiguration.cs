using Absence.Domain.Common;
using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

internal class AbsenceStatusEntityConfiguration : EntityConfiguration<AbsenceStatusEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceStatusEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .HasConversion<int>()
           .IsRequired();
    }
}