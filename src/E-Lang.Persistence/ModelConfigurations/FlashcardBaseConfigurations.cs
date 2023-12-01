using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class FlashcardBaseConfigurations
{
     public static ModelBuilder ConfigureFlashcardBase(this ModelBuilder modelBuilder)
     {
          modelBuilder.Entity<FlashcardBase>(entity =>
          {
               entity.HasIndex(e => e.WordOrPhrase);
               
               entity.HasIndex(e => e.Id);
               
               entity.HasMany(f => f.Meanings)
                    .WithMany()
                    .UsingEntity<FlashcardBaseMeaning>(
                    r => r
                         .HasOne<Meaning>()
                         .WithMany()
                         .HasForeignKey(e => e.MeaningId),
                    l => l
                         .HasOne<FlashcardBase>()
                         .WithMany()
                         .HasForeignKey(e => e.FlashcardBaseId));
          });
          
          return modelBuilder;
     }
}