using E_Lang.Application.Common.DTOs;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface ICollectionRepository : IRepository<Collection, CollectionCardDto>
{
    Task<IEnumerable<CollectionCardDto>> GetCollectionCardsAsync(Guid userId, Guid? parentCollectionId, CancellationToken cancellationToken);
    Task<CollectionDto?> GetCollectionAsDtoAsync(Guid collectionId, CancellationToken cancellationToken);
    Task<bool> IsUserCollectionOwner(Guid userId, Guid collectionId, CancellationToken cancellationToken);
}