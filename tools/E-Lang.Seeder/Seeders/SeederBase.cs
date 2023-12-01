using E_Lang.Application.Common.Interfaces;
using E_Lang.Builder.Builders;
using E_Lang.Seeder.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using IUserService = E_Lang.Seeder.Interfaces.IUserService;

namespace E_Lang.Seeder.Seeders;

public abstract class SeederBase : ISeeder
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IUserService _userService;

    protected BaseBuilder Builder => new BaseBuilder(_serviceProvider.GetRequiredService<IAppDbContext>());

    protected SeederBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _userService = serviceProvider.GetRequiredService<IUserService>();
    }
    
    public abstract Task Seed();
}