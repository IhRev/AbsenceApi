using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

internal class HolidayEntityConfiguration : EntityConfiguration<HolidayEntity, int>
{
    public override void Configure(EntityTypeBuilder<HolidayEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .HasMaxLength(30)
           .IsRequired();

        builder
            .Property(_ => _.StartDate)
            .IsRequired();

        builder
            .Property(_ => _.EndDate)
            .IsRequired();

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.Holidays)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}