using Absence.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public abstract class EntityConfiguration<TEntity, TId> 
    : IEntityTypeConfiguration<TEntity> where TEntity : class, IIdKeyed<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder
            .HasKey(_ => _.Id);

        builder
            .Property(_ => _.Id)
            .ValueGeneratedOnAdd();
    }
}