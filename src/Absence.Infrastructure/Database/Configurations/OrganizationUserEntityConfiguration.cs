using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class OrganizationUserEntityConfiguration : EntityConfiguration<OrganizationUserEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationUserEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(_ => _.IsAdmin)
            .IsRequired();

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.OrganizationsUsers)    
            .HasForeignKey(_ => _.UserId)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.OrganizationsUsers)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}