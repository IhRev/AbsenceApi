using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

internal class OrganizationUserInvitationEntityConfiguration : EntityConfiguration<OrganizationUserInvitationEntity, int>
{
    public override void Configure(EntityTypeBuilder<OrganizationUserInvitationEntity> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(_ => _.InvitedUser)
            .WithMany(_ => _.InvitationsReceived)
            .HasForeignKey(_ => _.Invited)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(_ => _.InviterUser)
            .WithMany(_ => _.InvitationsSent)
            .HasForeignKey(_ => _.Inviter)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.OrganizationUserInvitations)
            .HasForeignKey(_ => _.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}