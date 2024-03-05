using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class AttemptStageConfigurations
{
    public static ModelBuilder ConfigureAttemptStage(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttemptStage>(entity =>
        {
            entity.HasIndex(e => e.Id);

            entity.HasMany(a => a.Flashcards)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        return modelBuilder;
    }
}