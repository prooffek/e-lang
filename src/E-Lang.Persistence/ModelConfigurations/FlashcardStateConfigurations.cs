using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
            
            entity.HasDiscriminator<string>("Discriminator")
                .HasValue<InitFlashcardState>(nameof(InitFlashcardState));
        });

        modelBuilder.Entity<FlashcardState>().Property("Discriminator").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

        return modelBuilder;
    }
}