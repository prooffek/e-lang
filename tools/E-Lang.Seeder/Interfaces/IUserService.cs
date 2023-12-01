using E_Lang.Domain.Entities;

namespace E_Lang.Seeder.Interfaces;

public interface IUserService
{
    Guid GetUserId();
    
    User GetUser();
}