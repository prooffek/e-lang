using E_Lang.Application.Common.DTOs;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface ICollectionRepository : IRepository<Collection>, IRepositoryWithDto<Collection, CollectionCardDto>
{
    Task<Collection?> GetWithExtensionsByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<CollectionCardDto>> GetCollectionCardsAsync(Guid userId, Guid? parentCollectionId, CancellationToken cancellationToken);
    Task<CollectionDto?> GetCollectionAsDtoAsync(Guid collectionId, CancellationToken cancellationToken);
    Task<bool> IsUserCollectionOwnerAsync(Guid userId, Guid collectionId, CancellationToken cancellationToken);
    Task<bool> IsNameAlreadyUsedAsync(Guid userId, string collectionName, CancellationToken cancellationToken, Guid? collectionId = null);
    Task<IEnumerable<CollectionAutocompleteDto>> GetAutocompleteData(Guid userId, CancellationToken cancellationToken);
}