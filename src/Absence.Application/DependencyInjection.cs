using Absence.Application.Common.Adapters;
using Absence.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Absence.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services
            .AddAutoMapper(Assembly.GetExecutingAssembly());

        services
            .AddScoped<IRandomGenerator, RandomGenerator>();

        return services;
    }
}