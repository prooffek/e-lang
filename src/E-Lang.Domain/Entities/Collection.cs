using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities;

public class Collection : EntityBase
{
    [Required]
    [MaxLength(50)]
    [MinLength(50)]
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    public Collection? Parent { get; set; }
    public ICollection<Collection>? Subcollections { get; set; }
    
    public ICollection<Flashcard>? Flashcards { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
}