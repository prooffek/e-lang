using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using MediatR;

namespace E_Lang.Application.Collections.Commands;

public record DeleteCollectionRequest : IRequest
{
    public Guid? Id { get; set; }
}

public class DeleteCollectionRequestHandler : IRequestHandler<DeleteCollectionRequest>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IUserService _userService;

    public DeleteCollectionRequestHandler(ICollectionRepository collectionRepository, IUserService userService)
    {
        _collectionRepository = collectionRepository;
        _userService = userService;
    }

    public async Task<Unit> Handle(DeleteCollectionRequest request, CancellationToken cancellationToken)
    {
        if (!request.Id.HasValue || request.Id.Value == Guid.Empty)
            throw new ArgumentNullException(nameof(Collection.Id), "Collection id cannot be null or empty.");
        
        var collection = await _collectionRepository.GetWithExtensionsByIdAsync(request.Id.Value, cancellationToken)
            ?? throw new ArgumentException($"Collection with id {request.Id.Value.ToString()} not found.");

        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new ArgumentException("User not found.");

        if (user.Id != collection.OwnerId)
        {
            throw new UnauthorizedAccessException($"Access denied. User with id ${user.Id} is not allowed to remove the collection.");
        }

        if (collection.Subcollections is not null && collection.Subcollections.Count > 0)
            throw new Exception("Cannot delete collection. Delete its subcollections before proceeding.");
        
        _collectionRepository.Delete(collection);
        await _collectionRepository.SaveAsync(cancellationToken);

        return Unit.Value;
    }
}