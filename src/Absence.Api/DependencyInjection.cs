using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Absence.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigin",
                builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
            );
        });
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://absences/api",

                    ValidateAudience = true,
                    ValidAudience = "http://absences/api",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SymmetricSecurityKey")),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}