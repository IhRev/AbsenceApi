using Absence.Application.Common.Configurations;

namespace Absence.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
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
            .Configure<JwtConfiguration>(configuration.GetSection("JwtConfiguration"));

        return services;
    }
}