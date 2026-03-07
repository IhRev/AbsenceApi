using Absence.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations.Abstractions;

public class SoftDeleteEntityConfiguration<TEntity, TId> : EntityConfiguration<TEntity, TId>
    where TEntity : class, ISoftDelete, IIdKeyed<TId>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder
            .Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .HasQueryFilter(e => !e.IsDeleted);
    }
}