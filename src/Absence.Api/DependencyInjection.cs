using Absence.Api.Common.Interfaces;
using Absence.Api.Common.Services;
using Absence.Api.Services;
using Absence.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Absence.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services
            .AddControllers();

        services
            .AddHttpContextAccessor();

        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services
            .AddEndpointsApiExplorer();

        services
            .AddOpenApi();

        services
            .AddExceptionHandler<GlobalExceptionHandler>();

        services
            .AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

        services
            .Configure<JwtConfiguration>(configuration.GetSection("JwtConfiguration"));

        services
            .AddScoped<IUser, CurrentUser>()
            .AddScoped<IAbsenceHolidayOverlapChecker, AbsenceHolidayOverlapChecker>();

        return services;
    }
}
