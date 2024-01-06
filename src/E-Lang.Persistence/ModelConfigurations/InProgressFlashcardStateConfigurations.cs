using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations
{
    public static class InProgressFlashcardStateConfigurations
    {
        public static ModelBuilder ConfigureInProgressFlashcardState(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InProgressFlashcardState>(entity =>
            {
                entity.HasOne(e => e.Flashcard)
                    .WithMany()
                    .HasForeignKey(fs => fs.FlashcardId);

                entity.HasMany(fs => fs.CompletedQuizTypes)
                    .WithMany()
                    .UsingEntity<CompletedQuizType>(
                        l => l.HasOne<QuizType>().WithMany().HasForeignKey(e => e.QuizTypeId),
                        r => r.HasOne<InProgressFlashcardState>().WithMany().HasForeignKey(e => e.FlashcardStateId));
            });

            return modelBuilder;
        }
    }
}
