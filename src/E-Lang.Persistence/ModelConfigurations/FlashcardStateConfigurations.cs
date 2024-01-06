using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class FlashcardStateConfigurations
{
    public static ModelBuilder ConfigureFlashcardState(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlashcardState>(entity =>
        {
            entity.HasIndex(e => e.Id);

            entity.HasOne(e => e.Flashcard)
                .WithMany()
                .HasForeignKey(fs => fs.FlashcardId);
            
            entity.HasDiscriminator<string>($"{nameof(FlashcardState)}_type")
                .HasValue<InitFlashcardState>(nameof(InitFlashcardState));
        });
        
        return modelBuilder;
    }
}