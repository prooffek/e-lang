using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.Interfaces
{
    public interface IAttemptStageService
    {
        AttemptStage GetInitAttemptStage(IEnumerable<Flashcard> allFlashcards, FlashcardOrder order, int maxFlashcards);
    }
}
