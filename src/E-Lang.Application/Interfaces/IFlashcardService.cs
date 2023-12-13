using E_Lang.Application.Common.DTOs;

namespace E_Lang.Application.Interfaces
{
    public interface IFlashcardService
    {
        Task RemoveUnusedMeanings(Guid flashcardBaseId, IEnumerable<AddOrUpdateMeaningDto> meanings, CancellationToken cancellationToken);
        Task RemoveUnusedFlashcardBase(Guid currentflashcardBaseId, Guid? prevFlashcardBaseId, CancellationToken cancellationToken);
        Task RemoveUnusedFlashcardBase(Guid flashcardId, Guid flashcardBaseId, CancellationToken cancellationToken);
        Task RemoveUnusedFlashcardBases(IEnumerable<Guid> flashcardBaseIds, CancellationToken cancellationToken);
    }
}
