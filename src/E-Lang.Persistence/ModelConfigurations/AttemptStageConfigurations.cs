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

            entity.HasMany(e => e.Flashcards)
                .WithMany()
                .UsingEntity<AttemptStageFlashcardState>(
                    r => r.HasOne<FlashcardState>().WithMany().HasForeignKey(e => e.FlashcardStateId),
                    l => l.HasOne<AttemptStage>().WithMany().HasForeignKey(e => e.AttemptStageId));

            entity.HasDiscriminator<string>($"{nameof(AttemptStage)}_type")
                .HasValue<InitAttemptStage>(nameof(InitAttemptStage));
        });
        
        return modelBuilder;
    }
}