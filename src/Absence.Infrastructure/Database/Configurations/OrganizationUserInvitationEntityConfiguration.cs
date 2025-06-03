using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

internal class OrganizationUserInvitationEntityConfiguration : EntityConfiguration<OrganizationUserInvitationEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationUserInvitationEntity> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(_ => _.User)
            .WithMany(_ => _.OrganizationUserInvitations)
            .HasForeignKey(_ => _.UserId)
            .HasPrincipalKey(_ => _.ShortId);

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.OrganizationUserInvitations)
            .HasForeignKey(_ => _.OrganizationId);
    }
}