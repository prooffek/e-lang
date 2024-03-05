using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.Interfaces
{
    public interface IAttemptStageService
    {
        AttemptStage GetAttemptStage(Guid attemptId, IEnumerable<Flashcard> allFlashcards, FlashcardOrder order, int maxFlashcards);
        Task<AttemptStage?> GetNextAttemptStage(Attempt attempt, CancellationToken cancellationToken);
        Task CompleteAttemptStage(Attempt attempt, CancellationToken cancellationToken);
    }
}
