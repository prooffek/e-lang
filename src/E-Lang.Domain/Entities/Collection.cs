using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities;

public class Collection : EntityBase
{
    public const int NAME_MIN_LENGTH = 1;
    public const int NAME_MAX_LENGTH = 120;
    
    [Required]
    [MaxLength(NAME_MAX_LENGTH)]
    [MinLength(NAME_MIN_LENGTH)]
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    public Collection? Parent { get; set; }
    public ICollection<Collection>? Subcollections { get; set; }
    
    public ICollection<Flashcard>? Flashcards { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
}