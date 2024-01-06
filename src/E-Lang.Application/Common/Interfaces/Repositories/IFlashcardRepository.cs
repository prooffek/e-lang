using E_Lang.Application.Common.DTOs;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface IFlashcardRepository : IRepository<Flashcard>, IRepositoryWithDto<Flashcard, FlashcardDto>
{
    Task<Guid?> GetFlashcardBaseIdAsync(Guid flashcardId, CancellationToken cancellationToken);
    Task<IEnumerable<Flashcard>> GetFlashcardsByIdAsync(HashSet<Guid> ids, CancellationToken cancellationToken);
    Task<IEnumerable<Flashcard>> GetFlashcardsByCollectionId(Guid collectionId, CancellationToken cancellationToken);
}