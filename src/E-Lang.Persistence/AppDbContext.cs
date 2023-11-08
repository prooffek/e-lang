using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Persistence.ModelConfigurations;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions option) : base(option)
    {
    }
    
    public DbSet<T> GetDbSet<T>() where T : class
    {
        return Set<T>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureCollections();
        modelBuilder.ConfigureFlashcards();
        modelBuilder.ConfigureUser();
        
        base.OnModelCreating(modelBuilder);
    }
}