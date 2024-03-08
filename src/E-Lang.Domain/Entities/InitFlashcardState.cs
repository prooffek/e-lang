using E_Lang.Domain.Models;

namespace E_Lang.Domain.Entities;

public class InitFlashcardState : FlashcardState
{
    public InitFlashcardState()
    {
    }

    public InitFlashcardState(Flashcard flashcard) : base(flashcard) { }

    public override QuizType GetQuiz(Attempt attempt)
    {
        return attempt.QuizTypes?.FirstOrDefault(x => x.IsFirst)
            ?? throw new NullReferenceException("Default first quiz not found.");
    }

    public override FlashcardState GetNextState(NextStateData data)
    {
        if (!data.IsAnswerCorrect.HasValue || data.Attempt is null)
            return new InProgressFlashcardState(this, data.UtcNow);

        var flashcardState = new InProgressFlashcardState(this, data);

        return flashcardState.IsCompleted(data.Attempt)
            ? new CompletedFlashcardState(flashcardState, data.UtcNow)
            : flashcardState;
    }
}