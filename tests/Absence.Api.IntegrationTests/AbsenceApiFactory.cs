using Absence.Infrastructure.Database.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Absence.Api.IntegrationTests;

/// <summary>
/// Hosts the real API against a dedicated SQL Server database. Configuration is supplied here
/// rather than read from appsettings or user secrets so a run does not depend on the developer's
/// machine state.
/// </summary>
public class AbsenceApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string ConnectionString =
        @"Server=.\SQLEXPRESS;Database=AbsenceDB_Tests;Integrated Security=True;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:AbsenceDB"] = ConnectionString,
                ["JwtConfiguration:Issuer"] = "absence-tests",
                ["JwtConfiguration:Audience"] = "absence-tests",
                // Test-only signing key. HS256 requires at least 256 bits.
                ["JwtConfiguration:Secret"] = "integration-test-signing-key-which-is-long-enough",
                ["JwtConfiguration:JwtTokenExpireTimeInMinutes"] = "15",
                ["JwtConfiguration:RefreshTokenExpireTimeInDays"] = "7"
            });
        });
    }

    // Explicit implementation: the base class already exposes a ValueTask-returning DisposeAsync,
    // which would otherwise clash with xUnit's Task-returning one.
    async Task IAsyncLifetime.InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AbsenceContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}

[CollectionDefinition(nameof(AbsenceApiCollection))]
public class AbsenceApiCollection : ICollectionFixture<AbsenceApiFactory>;