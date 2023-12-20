using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class CustomPropertyRelation : EntityBase
    {
        [Required]
        public Guid RelationTypeId { get; set; }
        public RelationType? RelationType { get; set; }

        [Required]
        public Guid Property1Id { get; set; }

        [Required]
        public Guid Property2Id { get; set; }
    }
}
