using AbsenceApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbsenceApi.Database.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .ToTable("Users");

        builder
            .Property(_ => _.FirstName)
            .IsRequired();

        builder
            .Property(_ => _.SecondName)
            .IsRequired();

        builder
            .HasOne(_ => _.Organization)
            .WithMany(_ => _.Members)
            .HasForeignKey(_ => _.OrganizationId);

        builder
            .Property(u => u.Email)
            .IsRequired();
    }
}