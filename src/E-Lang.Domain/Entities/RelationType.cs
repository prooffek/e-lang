using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class RelationType : EntityBase
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public Guid OwnerId { get; set; }
    }
}
