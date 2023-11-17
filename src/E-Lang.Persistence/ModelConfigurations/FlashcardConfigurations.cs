using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class FlashcardConfigurations
{
    public static ModelBuilder ConfigureFlashcards(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.HasIndex(fc => 
                new {fc.Id, fc.OwnerId},
                $"{nameof(Flashcard)}_{nameof(Flashcard.Id)}_{nameof(Flashcard.OwnerId)}");
        });
        
        return modelBuilder;
    }
}