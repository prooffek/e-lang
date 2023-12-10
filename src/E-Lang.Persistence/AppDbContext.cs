using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Persistence.ModelConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace E_Lang.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions option, IDateTimeProvider dateTimeProvider) : base(option)
    {
        _dateTimeProvider = dateTimeProvider;
    }
    
    public DbSet<T> GetDbSet<T>() where T : class
    {
        return Set<T>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = _dateTimeProvider.UtcNow;
                    entry.Entity.ModifiedOn = _dateTimeProvider.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.CreatedOn = entry.GetDatabaseValues()!.GetValue<DateTime>(nameof(EntityBase.CreatedOn));
                    entry.Entity.ModifiedOn = _dateTimeProvider.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureCollections();
        modelBuilder.ConfigureFlashcards();
        modelBuilder.ConfigureUser();
        
        base.OnModelCreating(modelBuilder);
    }
}