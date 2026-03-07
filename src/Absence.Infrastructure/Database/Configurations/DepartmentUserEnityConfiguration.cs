using Absence.Domain.Entities;
using Absence.Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class DepartmentUserEnityConfiguration : EntityConfiguration<DepartmentUserEntity, int>
{
    public override void Configure(EntityTypeBuilder<DepartmentUserEntity> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(x => x.Department)
            .WithMany(x => x.DepartmentUsers)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.DepartmentUsers)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(_ => _.ShortId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}