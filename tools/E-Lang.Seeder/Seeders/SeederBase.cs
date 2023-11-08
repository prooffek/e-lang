using E_Lang.Application.Common.Interfaces;
using E_Lang.Builder.Builders;
using E_Lang.Seeder.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Seeder.Seeders;

public abstract class SeederBase : ISeeder
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IConfiguration _config;

    protected BaseBuilder Builder => new BaseBuilder(_serviceProvider.GetRequiredService<IAppDbContext>());

    protected SeederBase(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }
    
    public abstract Task Seed();
}