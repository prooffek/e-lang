using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities;

public class SeenQuizType : EntityBase
{
    [Required]
    public Guid QuizTypeId { get; set; }

    [Required]
    public Guid InProgressFlashcardStateId { get; set; }

    public SeenQuizType()
    {
    }

    public SeenQuizType(Guid flashcardStateId, Guid quizTypeId)
    {
        QuizTypeId = quizTypeId;
        InProgressFlashcardStateId = flashcardStateId;
    }
}