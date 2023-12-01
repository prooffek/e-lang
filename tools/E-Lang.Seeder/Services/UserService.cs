using E_Lang.Domain.Entities;
using E_Lang.Seeder.Interfaces;
using Microsoft.Extensions.Configuration;

namespace E_Lang.Seeder.Services;

public class UserService : IUserService
{
    private readonly IConfiguration _config;

    public UserService(IConfiguration config)
    {
        _config = config;
    }
    
    public Guid GetUserId()
    {
        return new Guid(_config.GetSection("SeederSettings:DefaultUser:Id").Value!);
    }

    public User GetUser()
    {
        return _config.GetSection("SeederSettings:DefaultUser").Get<User>()
               ?? new User
               {
                   Id = Guid.NewGuid(),
                   UserName = "Admin",
                   Email = "admin@e-lang.com"
               };
    }
}