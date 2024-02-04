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
        return data.IsAnswerCorrect.HasValue 
            ? new InProgressFlashcardState(this, data) 
            : new InProgressFlashcardState(this, data.UtcNow);
    }
}