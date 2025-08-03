using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class AbsenceEntityConfiguration : EntityConfiguration<AbsenceEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceEntity> builder)
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
            .HasOne(_ => _.AbsenceType)
            .WithMany(_ => _.Absences)
            .HasForeignKey(_ => _.AbsenceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.Absences)
            .HasForeignKey(_ => _.UserId)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
           .HasOne(_ => _.Organization)
           .WithMany(_ => _.Absences)
           .HasForeignKey(_ => _.OrganizationId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}