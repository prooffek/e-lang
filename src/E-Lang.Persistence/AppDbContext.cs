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
    public DbSet<Attempt> Attempts { get; set; }
    public DbSet<AttemptStage> AttemptStages { get; set; }
    public DbSet<CustomProperty> CustomProperties { get; set; }
    public DbSet<FlashcardState> FlashcardStates { get; set; }
    public DbSet<InitFlashcardState> InitFlashcardStates { get; set; }
    public DbSet<InProgressFlashcardState> InProgressFlashcardStates { get; set; }
    public DbSet<CompletedFlashcardState> CompletedFlashcardStates { get; set; }
    public DbSet<MeaningsRelation> MeaningsRelations { get; set; }
    public DbSet<QuizType> QuizTypes { get; set; }
    public DbSet<RelationType> RelationTypes { get; set; }
    public DbSet<AttemptProperty> AttemptProperties { get; set; }
    public DbSet<AttemptQuizType> AttemptQuizTypes { get; set; }
    public DbSet<CompletedQuizType> CompletedQuizTypes { get; set; }
    public DbSet<SeenQuizType> SeenQuizTypes { get; set; }
    public DbSet<CompletedFlashcard> CompletedFlashcards { get; set; }

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
                    entry.Entity.CreatedOn = entry.Entity.CreatedOn != DateTime.MinValue 
                        ? entry.Entity.CreatedOn 
                        : entry.GetDatabaseValues()!.GetValue<DateTime>(nameof(EntityBase.CreatedOn));
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
        modelBuilder.ConfigureFlashcardState();
        modelBuilder.ConfigureAttempts();
        modelBuilder.ConfigureAttemptStage();
        modelBuilder.ConfigureInProgressFlashcardState();
        modelBuilder.ConfigureQuizTypes();

        base.OnModelCreating(modelBuilder);
    }
}