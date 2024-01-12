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
            });

            return modelBuilder;
        }
    }
}
