using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class AttemptProperty : EntityBase
    {
        [Required]
        public Guid CustomPropertyId { get; set; }
        
        [Required]
        public Guid AttemptId { get; set; }
    }
}