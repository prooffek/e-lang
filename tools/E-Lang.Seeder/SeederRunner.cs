using E_Lang.Application.Common.Interfaces;
using E_Lang.Persistence;
using E_Lang.Seeder.Interfaces;
using E_Lang.Seeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IUserService = E_Lang.Seeder.Interfaces.IUserService;

namespace E_Lang.Seeder;

public class SeederRunner : ISeederRunner
{
    private readonly IServiceProvider _serviceProvider;

    public SeederRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task Run()
    {
        HandleDatabase();
        
        await new UserSeeder(_serviceProvider).Seed();
        await new CollectionSeeder(_serviceProvider).Seed();
        await new FlashcardsSeeder(_serviceProvider).Seed();
    }

    private void HandleDatabase()
    {
        var context = (AppDbContext) _serviceProvider.GetRequiredService<IAppDbContext>();
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
}