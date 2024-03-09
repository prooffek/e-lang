using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class ExcludedQuizType : EntityBase
    {
        [Required]
        public Guid InProgressFlashcardStateId { get; set; }

        [Required] 
        public Guid QuizTypeId { get; set; }

        public ExcludedQuizType()
        {
        }

        public ExcludedQuizType(Guid flashcardStateId, Guid quizTypeId)
        {
            InProgressFlashcardStateId = flashcardStateId;
            QuizTypeId = quizTypeId;
        }
    }
}
