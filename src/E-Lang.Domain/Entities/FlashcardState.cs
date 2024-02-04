using E_Lang.Domain.Models;

namespace E_Lang.Domain.Entities
{
    public abstract class FlashcardState : EntityBase
    {
        public Guid? FlashcardId { get; set; }
        public Flashcard? Flashcard { get; set; }

        public FlashcardState()
        {
        }

        public FlashcardState(Flashcard flashcard)
        {
            FlashcardId = flashcard.Id;
            Flashcard = flashcard;
        }

        public abstract FlashcardState GetNextState(NextStateData data);

        public abstract QuizType GetQuiz(Attempt attempt);
    }
}
