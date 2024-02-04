using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class CompletedQuizType : EntityBase
    {
        [Required]
        public Guid QuizTypeId { get; set; }

        [Required]
        public Guid InProgressFlashcardStateId { get; set; }

        public CompletedQuizType()
        {
        }

        public CompletedQuizType(Guid flashcardStateId, Guid quizTypeId)
        {
            QuizTypeId = quizTypeId;
            InProgressFlashcardStateId = flashcardStateId;
        }
    }
}
