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

        // Validated at startup: an unset secret or a zero lifetime otherwise surfaces much later
        // as tokens that are rejected the moment they are issued.
        services
            .AddOptions<JwtConfiguration>()
            .Bind(configuration.GetSection("JwtConfiguration"))
            .Validate(_ => !string.IsNullOrWhiteSpace(_.Secret), "JwtConfiguration:Secret must be configured.")
            .Validate(_ => !string.IsNullOrWhiteSpace(_.Issuer), "JwtConfiguration:Issuer must be configured.")
            .Validate(_ => !string.IsNullOrWhiteSpace(_.Audience), "JwtConfiguration:Audience must be configured.")
            .Validate(_ => _.JwtTokenExpireTimeInMinutes > 0, "JwtConfiguration:JwtTokenExpireTimeInMinutes must be greater than zero.")
            .Validate(_ => _.RefreshTokenExpireTimeInDays > 0, "JwtConfiguration:RefreshTokenExpireTimeInDays must be greater than zero.")
            .ValidateOnStart();

        services
            .AddScoped<IUser, CurrentUser>()
            .AddScoped<IAbsenceHolidayOverlapChecker, AbsenceHolidayOverlapChecker>()
            .AddScoped<IOrganizationAccess, OrganizationAccess>();

        return services;
    }
}
