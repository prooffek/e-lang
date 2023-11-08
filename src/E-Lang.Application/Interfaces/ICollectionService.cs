namespace E_Lang.Application.Interfaces;

public interface ICollectionService
{
    Task ValidateUserCollectionId(Guid userId, Guid? collectionId, CancellationToken cancellationToken);
}