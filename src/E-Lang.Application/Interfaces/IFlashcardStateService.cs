using E_Lang.Domain.Entities;

namespace E_Lang.Application.Interfaces
{
    public interface IFlashcardStateService
    {
        Task UpdateFlashcardState(Guid flashcardStateId, bool isAnswerCorrect, Attempt attempt, CancellationToken cancellationToken);
    }
}
