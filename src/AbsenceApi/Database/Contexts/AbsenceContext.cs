using AbsenceApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AbsenceApi.Database.Contexts;

public class AbsenceContext(DbContextOptions options) : IdentityDbContext(options)
{
    public virtual DbSet<AbsenceEntity> Absences { get; set; }
    public virtual DbSet<AbsenceTypeEntity> AbsenceTypes { get; set; }
    public virtual DbSet<OrganizationEntity> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AbsenceContext).Assembly);
    }
}