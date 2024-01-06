using E_Lang.Application.Common.DTOs;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Interfaces
{
    public interface IFlashcardService
    {
        Task RemoveUnusedMeanings(Guid flashcardBaseId, IEnumerable<AddOrUpdateMeaningDto> meanings, CancellationToken cancellationToken);
        Task RemoveUnusedFlashcardBase(Guid currentFlashcardBaseId, Guid? prevFlashcardBaseId, CancellationToken cancellationToken);
        Task RemoveUnusedFlashcardBase(Guid flashcardId, FlashcardBase? flashcardBase, CancellationToken cancellationToken);
        Task RemoveUnusedFlashcardBases(IEnumerable<Guid> flashcardBaseIds, CancellationToken cancellationToken);
    }
}
