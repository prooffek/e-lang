using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Seeder.Seeders;

public class UserSeeder : SeederBase
{
    private readonly IUserRepository _userRepository;

    public UserSeeder(IServiceProvider serviceProvider, IConfiguration config)
        : base(serviceProvider, config)
    {
        _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
    }
    
    public override async Task Seed()
    {
        await SeedUsers();
    }

    private async Task SeedUsers()
    {
        var users = GetUsers();
        _userRepository.AddRange(users);
        await _userRepository.SaveAsync();
    }

    private IEnumerable<User> GetUsers()
    {
        return new List<User>()
        {
            GetAdminUser()
        };
    }

    private User GetAdminUser()
    {
        var admin = _config.GetSection("SeederSettings:DefaultUser").Get<User>()
                    ?? new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "Admin",
                        Email = "admin@e-lang.com"
                    };
        
        admin.CreatedOn = DateTime.UtcNow;
        admin.ModifiedOn = DateTime.UtcNow;

        return admin;
    }
}