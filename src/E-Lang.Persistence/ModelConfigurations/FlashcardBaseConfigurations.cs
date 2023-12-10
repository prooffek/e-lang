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
                    .WithOne(m => m.FlashcardBase)
                    .HasForeignKey(m => m.FlashcardBaseId);
          });
          
          return modelBuilder;
     }
}