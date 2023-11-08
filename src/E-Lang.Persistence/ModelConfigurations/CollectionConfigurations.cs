using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class CollectionConfigurations
{
    public static ModelBuilder ConfigureCollections(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasIndex(e =>
                    new {e.Id, e.ParentId},
                $"{nameof(Collection)}_{nameof(Collection.Id)}_{nameof(Collection.ParentId)}");

            entity.HasMany(c => c.Flashcards)
                .WithMany(fc => fc.Collections)
                .UsingEntity<CollectionFlashcard>(
                    r => r
                        .HasOne<Flashcard>()
                        .WithMany()
                        .HasForeignKey(e => e.FlashcardId),
                    l => l
                        .HasOne<Collection>()
                        .WithMany()
                        .HasForeignKey(e => e.CollectionId));
        });

        return modelBuilder;
    }
}