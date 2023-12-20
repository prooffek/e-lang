using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class AttemptStageFlashcardState : EntityBase
    {
        [Required]
        public Guid AttemptStageId { get; set; }
        
        [Required]
        public Guid FlashcardStateId { get; set; }
    }
}
