using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace E_Lang.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<Collection> Collections { get; }
    public DbSet<Flashcard> Flashcards { get; }
    public DbSet<User> Users { get; }
    public DbSet<Attempt> Attempts { get; }
    public DbSet<AttemptStage> AttemptStages { get; }
    public DbSet<CustomProperty> CustomProperties { get; }
    public DbSet<FlashcardState> FlashcardStates { get; }
    public DbSet<InitFlashcardState> InitFlashcardStates { get; }
    public DbSet<InProgressFlashcardState> InProgressFlashcardStates { get; }
    public DbSet<MeaningsRelation> MeaningsRelations { get; }
    public DbSet<QuizType> QuizTypes { get; }
    public DbSet<RelationType> RelationTypes { get; }
    public DbSet<AttemptProperty> AttemptProperties { get; }
    public DbSet<AttemptQuizType> AttemptQuizTypes { get; }
    public DbSet<CompletedQuizType> CompletedQuizTypes { get; }
    public DbSet<SeenQuizType> SeenQuizTypes { get; }
    public DbSet<CompletedFlashcardState> CompletedFlashcardStates { get; }


    DbSet<T> GetDbSet<T>() where T : class;

    EntityEntry<T> Entry<T>(T entity) where T : class;

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}