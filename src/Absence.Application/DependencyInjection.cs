using Absence.Application.Behaviors;
using Absence.Application.Common.Adapters;
using Absence.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Absence.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PermissionValidationBehavior<,>));

        services
            .AddAutoMapper(Assembly.GetExecutingAssembly());

        services
            .AddScoped<IRandomGenerator, RandomGenerator>();

        return services;
    }
}