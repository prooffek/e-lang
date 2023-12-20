using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations
{
    public static class AttemptConfigurations
    {
        public static ModelBuilder ConfigureAttempts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attempt>(entity =>
            {
                entity.HasIndex(e => e.Id);
                entity.HasIndex(e => e.CollectionId);

                entity.HasOne(a => a.Collection)
                    .WithMany(c => c.Attempts)
                    .HasForeignKey(a => a.CollectionId);

                entity.HasOne(a => a.CurrentStage)
                    .WithOne()
                    .HasForeignKey<Attempt>(e => e.CurrentStageId);

                entity.HasMany(a => a.CompletedFlashcards)
                    .WithMany()
                    .UsingEntity<CompletedFlashcard>(
                        r => r.HasOne<Flashcard>().WithMany().HasForeignKey(c => c.FlashcardId),
                        l => l.HasOne<Attempt>().WithMany().HasForeignKey(c => c.AttemptId));
                
                entity.HasMany(e => e.Properties)
                    .WithMany()
                    .UsingEntity<AttemptProperty>(
                        r => r.HasOne<CustomProperty>().WithMany().HasForeignKey(e => e.CustomPropertyId),
                        l => l.HasOne<Attempt>().WithMany().HasForeignKey(e => e.AttemptId));

                entity.HasMany(e => e.QuizTypes)
                    .WithMany()
                    .UsingEntity<AttemptQuizType>(
                        r => r.HasOne<QuizType>().WithMany().HasForeignKey(e => e.QuiTypeId),
                        l => l.HasOne<Attempt>().WithMany().HasForeignKey(e => e.AttemptId));
            });

            return modelBuilder;
        }
    }
}
