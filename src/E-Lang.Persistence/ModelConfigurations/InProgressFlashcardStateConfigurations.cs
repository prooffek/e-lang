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
                    .WithOne()
                    .HasForeignKey(x => x.InProgressFlashcardStateId);

                entity.HasMany(fs => fs.SeenQuizTypes)
                    .WithOne()
                    .HasForeignKey(x => x.InProgressFlashcardStateId);
            });

            return modelBuilder;
        }
    }
}
