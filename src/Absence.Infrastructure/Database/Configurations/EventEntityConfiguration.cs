using Absence.Domain.Entities;
using Absence.Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

internal class EventEntityConfiguration : EntityConfiguration<EventEntity, int>
{
    public override void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .HasMaxLength(30)
           .IsRequired();

        builder
            .Property(_ => _.Date)
            .IsRequired();

        builder
            .Property(_ => _.NonWorkingDay)
            .IsRequired();

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.Events)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}