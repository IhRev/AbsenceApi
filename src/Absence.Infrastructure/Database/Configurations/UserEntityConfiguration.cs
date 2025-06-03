using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absence.Infrastructure.Database.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .ToTable("Users");

        builder
            .HasAlternateKey(_ => _.ShortId);

        builder
            .Property(_ => _.ShortId)
            .ValueGeneratedOnAdd();

        builder
            .Property(_ => _.RefreshTokenExpiresAt)
            .IsRequired(false);

        builder
            .Property(_ => _.RefreshToken)
            .IsRequired(false);

        builder
            .Property(_ => _.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(_ => _.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();
    }
}