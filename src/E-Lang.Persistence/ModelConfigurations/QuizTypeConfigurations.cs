using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations
{
    public static class QuizTypeConfigurations
    {
        public static ModelBuilder ConfigureQuizTypes(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizType>(entity =>
            {
                entity.HasIndex(x => x.Id);
                entity.HasIndex(x => x.OwnerId);

                entity.HasMany<CompletedQuizType>()
                    .WithOne()
                    .HasForeignKey(x => x.QuizTypeId);

                entity.HasMany<SeenQuizType>()
                    .WithOne()
                    .HasForeignKey(x => x.QuizTypeId);

                entity.HasMany<ExcludedQuizType>()
                    .WithOne()
                    .HasForeignKey(x => x.QuizTypeId);
            });

            return modelBuilder;
        }
    }
}
