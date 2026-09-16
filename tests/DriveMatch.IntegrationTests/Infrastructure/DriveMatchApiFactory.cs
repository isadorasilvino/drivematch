using DriveMatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace DriveMatch.IntegrationTests.Infrastructure;

public sealed class DriveMatchApiFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:17")
            .WithDatabase("drivematch_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            _postgres.GetConnectionString());

        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            "DriveMatchIntegrationTestsSecretKey12345678901234567890");

        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            "DriveMatch.IntegrationTests");

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            "DriveMatch.IntegrationTests");

        Environment.SetEnvironmentVariable(
            "Scheduling__TimeZoneId",
            "America/Sao_Paulo");

        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<DriveMatchDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            null);

        Environment.SetEnvironmentVariable(
            "Scheduling__TimeZoneId",
            null);

        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}