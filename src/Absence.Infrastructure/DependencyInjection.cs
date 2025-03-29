using Absence.Application.Common.Abstractions;
using Absence.Domain.Entities;
using Absence.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Absence.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IRepository<AbsenceEntity>, Repository<AbsenceEntity>>();
        services.AddScoped<IRepository<AbsenceTypeEntity>, Repository<AbsenceTypeEntity>>();
        services.AddScoped<IRepository<OrganizationEntity>, Repository<OrganizationEntity>>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}