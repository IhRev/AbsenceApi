using Absence.Application.Common.Configurations;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Infrastructure.Database.Contexts;
using Absence.Infrastructure.Database.Repositories;
using Absence.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            ArgumentNullException.ThrowIfNull(connectionString);

            opt.UseSqlServer(connectionString);
        });

        services
            .AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                var jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();
                ArgumentNullException.ThrowIfNull(jwtConfiguration);

                var key = Encoding.UTF8.GetBytes(jwtConfiguration.Secret);

                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services
            .AddDefaultIdentity<UserEntity>()
            .AddEntityFrameworkStores<AbsenceContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthorization();

        services
            .Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}