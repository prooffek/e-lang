using E_Lang.Application.Collections.Requests;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;

namespace E_Lang.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;

    public CollectionService(ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }
    
    public async Task ValidateUserCollectionId(Guid userId, Guid? collectionId, CancellationToken cancellationToken)
    {
        if (!collectionId.HasValue || collectionId.Value == Guid.Empty)
            throw new ArgumentNullException(nameof(GetCollectionRequest.CollectionId),
                "Parent collection Id cannot be null or empty.");

        if (!await _collectionRepository.IsUserCollectionOwner(userId, collectionId.Value,
                cancellationToken))
            throw new ArgumentException(
                $"User with Id {userId} does not have collection with Id {collectionId.Value}");
    }
}