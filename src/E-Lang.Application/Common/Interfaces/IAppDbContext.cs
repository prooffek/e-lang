using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<Collection> Collections { get; }
    public DbSet<Flashcard> Flashcards { get; }
    public DbSet<User> Users { get; }
    
    DbSet<T> GetDbSet<T>() where T : class;

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}