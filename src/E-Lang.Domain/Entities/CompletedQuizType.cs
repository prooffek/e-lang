using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class CompletedQuizType : EntityBase
    {
        [Required]
        public Guid QuizTypeId { get; set; }
        
        [Required]
        public Guid FlashcardStateId { get; set; }
    }
}
