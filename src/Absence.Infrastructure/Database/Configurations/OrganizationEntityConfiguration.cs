using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class OrganizationEntityConfiguration : EntityConfiguration<OrganizationEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(_ => _.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasOne(_ => _.Owner)
            .WithMany(_ => _.Organizations)
            .HasForeignKey(_ => _.OwnerId)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}