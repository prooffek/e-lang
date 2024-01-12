using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations
{
    public static class RelationTypeConfigurations
    {
        public static ModelBuilder ConfigureRelationType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelationType>(entity =>
            {
                entity.HasIndex(x => x.Id);
                entity.HasIndex(x => x.OwnerId);
            });

            return modelBuilder;
        }
    }
}
