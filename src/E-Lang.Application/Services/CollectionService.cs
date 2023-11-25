using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;

    public CollectionService(ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }
    
    public async Task ValidateUserCollectionId(Guid userId, Guid? collectionId, ActionTypes actionType, CancellationToken cancellationToken)
    {
        if (!collectionId.HasValue || collectionId.Value == Guid.Empty)
            throw new NullOrEmptyValidationException(nameof(Collection), nameof(Collection.Id), actionType);

        if (!await _collectionRepository.IsUserCollectionOwnerAsync(userId, collectionId.Value,
                cancellationToken))
            throw new NotFoundValidationException(nameof(Collection), nameof(Collection.Id), collectionId.Value.ToString());
    }
}