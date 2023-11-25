using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using MediatR;

namespace E_Lang.Application.Collections.Commands;

public record UpdateCollectionRequest : IRequest<CollectionDto?>
{
    public UpdateCollectionDto UpdateDto { get; set; }
}

public class UpdateCollectionRequestHandler : IRequestHandler<UpdateCollectionRequest, CollectionDto?>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserService _userService;

    public UpdateCollectionRequestHandler(ICollectionRepository collectionRepository,
        IDateTimeProvider dateTimeProvider, IUserService userService)
    {
        _collectionRepository = collectionRepository;
        _dateTimeProvider = dateTimeProvider;
        _userService = userService;
    }

    public async Task<CollectionDto?> Handle(UpdateCollectionRequest request, CancellationToken cancellationToken)
    {        
        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new UserNotFoundException();
        
        var collection = await _collectionRepository.GetByIdAsync(request.UpdateDto.Id, cancellationToken)
                         ?? throw new NotFoundValidationException(nameof(Collection), nameof(Collection.Name), request.UpdateDto.Name);

        collection.Name = request.UpdateDto.Name;
        collection.ParentId = request.UpdateDto.ParentCollectionId;
        collection.ModifiedOn = _dateTimeProvider.UtcNow;
        
        _collectionRepository.Update(collection);
        await _collectionRepository.SaveAsync(cancellationToken);

        return await _collectionRepository.GetCollectionAsDtoAsync(collection.Id, cancellationToken);
    }
}