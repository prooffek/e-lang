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

                entity.HasMany(a => a.CompletedFlashcards)
                    .WithMany()
                    .UsingEntity<CompletedFlashcard>();
                
                entity.HasMany(e => e.Properties)
                    .WithMany()
                    .UsingEntity<AttemptProperty>();

                entity.HasMany(e => e.QuizTypes)
                    .WithMany()
                    .UsingEntity<AttemptQuizType>();
            });

            return modelBuilder;
        }
    }
}
