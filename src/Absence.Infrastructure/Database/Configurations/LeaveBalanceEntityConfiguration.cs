using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class LeaveBalanceEntityConfiguration : EntityConfiguration<LeaveBalanceEntity, int>
{
    public override void Configure(EntityTypeBuilder<LeaveBalanceEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder
            .Property(x => x.TotalDays)
            .IsRequired();

        builder
            .Property(x => x.AvailableDays)
            .IsRequired();

        builder
            .Property(x => x.Year)
            .IsRequired();

        builder
            .HasOne(x => x.Organization)
            .WithMany(x => x.LeaveBalance)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.LeaveBalance)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}