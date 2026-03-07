using Absence.Domain.Entities;
using Absence.Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class AbsenceRequestEntityConfiguration : EntityConfiguration<AbsenceRequestEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceRequestEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(_ => _.RequestType)
            .HasConversion<int>()
            .IsRequired();

        builder
           .Property(_ => _.OldAbsenceTypeId)
           .IsRequired(false);

        builder
           .Property(_ => _.NewAbsenceTypeId)
           .IsRequired(false);

        builder
            .Property(_ => _.OldStartDate)
            .IsRequired(false);

        builder
            .Property(_ => _.NewStartDate)
            .IsRequired(false);

        builder
            .Property(_ => _.OldEndDate)
            .IsRequired(false);

        builder
            .Property(_ => _.NewEndDate)
            .IsRequired(false);

        builder
           .Property(_ => _.OldName)
           .HasMaxLength(30)
           .IsRequired(false);

        builder
           .Property(_ => _.NewName)
           .HasMaxLength(30)
           .IsRequired(false);

        builder
           .Property(_ => _.AbsenceId)
           .IsRequired(false);

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.AbsenceRequests)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.AbsenceRequests)
            .HasPrincipalKey(_ => _.ShortId)
            .HasForeignKey(_ => _.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}