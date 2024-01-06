namespace E_Lang.Domain.Entities;

public class InitFlashcardState : FlashcardState
{
    public InitFlashcardState()
    {
    }

    public InitFlashcardState(Flashcard flashcard)
    {
        FlashcardId = flashcard.Id;
        Flashcard = flashcard;
    }
}