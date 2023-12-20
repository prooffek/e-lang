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

            entity.HasIndex(e => e.CollectionId);

            entity.HasOne(f => f.FlashcardBase)
                .WithMany()
                .HasForeignKey(f => f.FlashcardBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(f => f.MeaningRelations)
                .WithOne()
                .HasForeignKey(r => r.Meaning1Id);

            entity.HasMany(f => f.MeaningRelations)
                .WithOne()
                .HasForeignKey(r => r.Meaning2Id);

            entity.HasMany(f => f.PropertyRelaions)
                .WithOne()
                .HasForeignKey(r => r.Property1Id);

            entity.HasMany(f => f.PropertyRelaions)
                .WithOne()
                .HasForeignKey(r => r.Property2Id);
        });
        
        return modelBuilder;
    }
}