using E_Lang.Application.Common.Enums;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Interfaces;

public interface ICollectionService
{
    Task ValidateUserCollectionId(Guid userId, Guid? collectionId, ActionTypes actionType, CancellationToken cancellationToken);
}