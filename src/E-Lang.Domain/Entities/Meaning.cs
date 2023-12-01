using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class Meaning : EntityBase
    {
        [Required]
        [MinLength(1)]
        [MaxLength(10000)]
        public string Value { get; set; }

        // If not null => the meaning is restricted to the collection specified
        // If null => the meaning is always loaded for the flashcard
        [Required]
        public Guid FlashcardId { get; set; }

        public ICollection<FlashcardBase> FlashcardBases { get; set; }
    }
}
