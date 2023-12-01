using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class CollectionConfigurations
{
    public static ModelBuilder ConfigureCollections(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasIndex(e => e.OwnerId);
            
            entity.HasIndex(e =>
                    new {e.Id, e.ParentId},
                $"{nameof(Collection)}_{nameof(Collection.Id)}_{nameof(Collection.ParentId)}");

            entity.HasMany(c => c.Flashcards)
                .WithOne(f => f.Collection)
                .HasForeignKey(f => f.CollectionId);

            entity.HasMany(c => c.Subcollections)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentId);
        });

        return modelBuilder;
    }
}