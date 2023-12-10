using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class Meaning : EntityBase
    {
        [Required]
        [MinLength(1)]
        [MaxLength(10000)]
        public string Value { get; set; }

        public Guid FlashcardBaseId { get; set; }

        public FlashcardBase? FlashcardBase { get; set; }
    }
}
