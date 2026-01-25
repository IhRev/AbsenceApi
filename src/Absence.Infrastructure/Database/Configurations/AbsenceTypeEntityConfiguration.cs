using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class AbsenceTypeEntityConfiguration : EntityConfiguration<AbsenceTypeEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceTypeEntity> builder)
    {
        base.Configure(builder);

        builder
           .Property(_ => _.Name)
           .HasMaxLength(30)
           .IsRequired();

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.AbsenceTypes)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}