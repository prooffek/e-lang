using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using E_Lang.Domain.Enums;

namespace E_Lang.Domain.Entities;

public class Flashcard : EntityBase
{
    public DateTime? LastStatusChangedOn { get; set; }

    public DateTime? LastSeenOn { get; set; }
    
    
    [Required]
    public Guid CollectionId { get; set; }

    public Collection? Collection { get; set; }


    [Required]
    public Guid FlashcardBaseId { get; set; }

    public FlashcardBase? FlashcardBase { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    public IEnumerable<MeaningsRelation>? MeaningRelations { get; set; }

    public ICollection<CustomPropertyRelation>? PropertyRelaions { get; set; }
}