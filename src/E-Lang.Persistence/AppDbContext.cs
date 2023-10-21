using E_Lang.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions option) : base(option)
    {
    }
    
    public DbSet<T> GetDbSet<T>() where T : class
    {
        return Set<T>();
    }
}