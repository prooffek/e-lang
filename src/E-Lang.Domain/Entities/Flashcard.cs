using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities;

public class Flashcard : EntityBase
{
    [Required]
    public Guid OwnerId { get; set; }
    
    public ICollection<Collection>? Collections { get; set; }
}