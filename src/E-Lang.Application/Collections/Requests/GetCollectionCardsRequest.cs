using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using MediatR;

namespace E_Lang.Application.Collections.Requests;

public record GetCollectionCardsRequest : IRequest<IEnumerable<CollectionCardDto>>
{
    public Guid? ParentCollectionId { get; set; }
}

public class GetUserCollectionsRequestHandler : IRequestHandler<GetCollectionCardsRequest, IEnumerable<CollectionCardDto>>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IUserService _userService;

    public GetUserCollectionsRequestHandler(ICollectionRepository collectionRepository, IUserService userService)
    {
        _collectionRepository = collectionRepository;
        _userService = userService;
    }
    
    public async Task<IEnumerable<CollectionCardDto>> Handle(GetCollectionCardsRequest request, CancellationToken cancellationToken)
    {
        var currentUser = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();

        return await _collectionRepository.GetCollectionCardsAsync(currentUser!.Id, request.ParentCollectionId, cancellationToken);
    }
}