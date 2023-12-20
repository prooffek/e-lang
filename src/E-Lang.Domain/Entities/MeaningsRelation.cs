using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class MeaningsRelation : EntityBase
    {
        [Required]
        public Guid RelationTypeId { get; set; }
        public RelationType? RelationType { get; set; }

        [Required]
        public Guid Meaning1Id { get; set; }

        [Required]
        public Guid Meaning2Id { get; set; }
    }
}
