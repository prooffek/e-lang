using E_Lang.Application.Interfaces;
using E_Lang.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace E_Lang.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public const string CONNECTION_STRING = "postgresDbConnectionString";
    
    private readonly IDateTimeProvider? _dateTimeProvider;

    public AppDbContextFactory()
    {
    }
    
    public AppDbContextFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }
    
    public AppDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var pathToWebApi = Directory.GetCurrentDirectory().Replace("Persistence", "WebApi");
        var config = new ConfigurationBuilder()
            .SetBasePath(pathToWebApi)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionBuilder = new DbContextOptionsBuilder();
        optionBuilder.UseNpgsql(config.GetConnectionString(CONNECTION_STRING));

        return new AppDbContext(optionBuilder.Options, _dateTimeProvider ?? new DateTimeProvider());
    }
}