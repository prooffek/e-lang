using E_Lang.Application.Common.Interfaces;
using E_Lang.Persistence;
using E_Lang.Seeder.Interfaces;
using E_Lang.Seeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Seeder;

public class SeederRunner : ISeederRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;

    public SeederRunner(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }
    
    public async Task Run()
    {
        HandleDatabase();
        
        await new UserSeeder(_serviceProvider, _config).Seed();
        await new CollectionSeeder(_serviceProvider, _config).Seed();
    }

    private void HandleDatabase()
    {
        var context = (AppDbContext) _serviceProvider.GetRequiredService<IAppDbContext>();
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
}