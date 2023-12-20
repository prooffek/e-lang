using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations
{
    public static class CustomPropertyRelationConfigurations
    {
        public static ModelBuilder ConfigureCustomPropertyRelation(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomPropertyRelation>(entity =>
            {
                entity.HasIndex(x => x.Property1Id);
                entity.HasIndex(x => x.Property2Id);

                entity.HasOne(e => e.RelationType)
                .WithMany()
                .HasForeignKey(e => e.RelationTypeId);
            });

            return modelBuilder;
        }
    }
}
