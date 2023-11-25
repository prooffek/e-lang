using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
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
            throw new NullOrEmptyValidationException(nameof(Collection), nameof(Collection.Id), ActionTypes.Delete);
        
        var collection = await _collectionRepository.GetWithExtensionsByIdAsync(request.Id.Value, cancellationToken)
            ?? throw new NotFoundValidationException(nameof(Collection), nameof(Collection.Id), request.Id.Value.ToString());

        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new UserNotFoundException();

        if (user.Id != collection.OwnerId)
        {
            throw new UnauthorizedException(user.Id, ActionTypes.Delete);
        }

        if (collection.Subcollections is not null && collection.Subcollections.Count > 0)
            throw new RelatedRecordValidationException(nameof(Collection), "Delete all subcollections and try again.");
        
        _collectionRepository.Delete(collection);
        await _collectionRepository.SaveAsync(cancellationToken);

        return Unit.Value;
    }
}