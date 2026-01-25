using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class AbsenceEventEntityConfiguration : EntityConfiguration<AbsenceEventEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceEventEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.AbsenceId)
           .IsRequired(false);

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
           .Property(_ => _.AbsenceTypeId)
           .IsRequired();

        builder
            .Property(_ => _.AbsenceEventType)
            .HasConversion<int>()
            .IsRequired();

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.AbsenceEvents)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.AbsenceEvents)
            .HasPrincipalKey(_ => _.ShortId)
            .HasForeignKey(_ => _.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}