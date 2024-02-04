using E_Lang.Domain.Enums;
using E_Lang.Domain.Models;

namespace E_Lang.Domain.Entities;

public class CompletedFlashcardState : FlashcardState
{
    public CompletedFlashcardState()
    {
    }

    public CompletedFlashcardState(Flashcard flashcard) : base(flashcard) 
    { 
    }

    public CompletedFlashcardState(FlashcardState flashcardState, DateTime utcNow)
    {
        Id = flashcardState.Id;
        CreatedOn = flashcardState.CreatedOn;
        FlashcardId = flashcardState.FlashcardId;
        Flashcard = flashcardState.Flashcard;
        Flashcard.LastStatusChangedOn = utcNow;
        Flashcard.LastSeenOn = utcNow;
    }

    public override QuizType GetQuiz(Attempt attempt)
    {
        throw new Exception("Flashcard is already completed");
    }

    public override FlashcardState GetNextState(NextStateData data)
    {
        throw new Exception("Flashcard is already completed");
    }
}