using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class CompletedFlashcard : EntityBase
    {
        [Required]
        public Guid FlashcardId { get; set; }

        [Required]
        public Guid AttemptId { get; set; }
    }
}
