using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class FlashcardBase : EntityBase
    {
        [Required]
        [MinLength(1)]
        [MaxLength(10000)]
        public string WordOrPhrase { get; set; }

        public ICollection<Meaning> Meanings { get; set; }
    }
}
