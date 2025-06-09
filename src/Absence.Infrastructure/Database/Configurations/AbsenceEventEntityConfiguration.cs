using Absence.Domain.Entities;
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
           .IsRequired(false);

        builder
            .Property(_ => _.StartDate)
            .IsRequired(false);

        builder
            .Property(_ => _.EndDate)
            .IsRequired(false);

        builder
           .Property(_ => _.AbsenceTypeId)
           .IsRequired(false);

        builder
           .Property(_ => _.UserId)
           .IsRequired(false);

        builder
            .HasOne(_ => _.AbsenceEventType)
            .WithMany(_ => _.AbsenceEvents)
            .HasForeignKey(_ => _.AbsenceEventTypeId);

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.AbsenceEvents)
            .HasForeignKey(_ => _.OrganizationId);

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.AbsenceEvents)
            .HasPrincipalKey(_ => _.ShortId)
            .HasForeignKey(_ => _.UserId);
    }
}