using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class AbsenceTypeEntityConfiguration : SoftDeleteEntityConfiguration<AbsenceTypeEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceTypeEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .HasMaxLength(30)
           .IsRequired();

        builder
           .Property(_ => _.Code)
           .HasMaxLength(5)
           .IsRequired();

        builder
           .Property(_ => _.RequiresApproval)
           .IsRequired();

        builder
           .Property(_ => _.CountsTowardAnnualLeave)
           .IsRequired();

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.AbsenceTypes)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}