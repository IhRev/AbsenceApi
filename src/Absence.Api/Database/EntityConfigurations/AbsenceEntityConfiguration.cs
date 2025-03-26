using AbsenceApi.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsenceApi.Database.EntityConfigurations;

public class AbsenceEntityConfiguration : EntityConfiguration<AbsenceEntity, int>
{
    public override void Configure(EntityTypeBuilder<AbsenceEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(_ => _.Name)
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
            .HasForeignKey(_ => _.AbsenceTypeId);

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.Absences)
            .HasForeignKey(_ => _.UserId);
    }
}