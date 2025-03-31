using Absence.Application.Common.Abstractions;
using Absence.Domain.Entities;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Absence.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        string connectionStringName)
    {
        services.AddDbContext<AbsenceContext>(opt => 
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);
            opt.UseSqlServer(connectionString);
        });

        services
            .AddDefaultIdentity<UserEntity>()
            .AddEntityFrameworkStores<AbsenceContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IRepository<AbsenceEntity>, Repository<AbsenceEntity>>();
        services.AddScoped<IRepository<AbsenceTypeEntity>, Repository<AbsenceTypeEntity>>();
        services.AddScoped<IRepository<OrganizationEntity>, Repository<OrganizationEntity>>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}