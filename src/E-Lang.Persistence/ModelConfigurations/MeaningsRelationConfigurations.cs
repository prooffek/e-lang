using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.ModelConfigurations;

public static class MeaningsRelationConfigurations
{
    public static ModelBuilder ConfigureMeaningRelations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeaningsRelation>(entity =>
        {
            entity.HasIndex(e => e.Meaning1Id);
            entity.HasIndex(e => e.Meaning2Id);

            entity.HasOne(e => e.RelationType)
                .WithMany()
                .HasForeignKey(e => e.RelationTypeId);
        });
        
        return modelBuilder;
    }
}