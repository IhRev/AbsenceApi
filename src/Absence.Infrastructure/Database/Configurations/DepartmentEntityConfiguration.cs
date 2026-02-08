using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class DepartmentEntityConfiguration : SoftDeleteEntityConfiguration<DepartmentEntity, int>
{
    public override void Configure(EntityTypeBuilder<DepartmentEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(x => x.Name)
            .IsRequired();

        builder.HasOne(x => x.Organization)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}