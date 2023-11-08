using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using MediatR;

namespace E_Lang.Application.Collections.Requests;

public record GetCollectionCardsRequest : IRequest<IEnumerable<CollectionCardDto>>
{
    public Guid? ParentCollectionId { get; set; }
}

public class GetUserCollectionsRequestHandler : IRequestHandler<GetCollectionCardsRequest, IEnumerable<CollectionCardDto>>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IUserValidationService _userValidationService;
    private readonly IUserService _userService;

    public GetUserCollectionsRequestHandler(ICollectionRepository collectionRepository, 
        IUserValidationService userValidationService, IUserService userService)
    {
        _collectionRepository = collectionRepository;
        _userValidationService = userValidationService;
        _userService = userService;
    }
    
    public async Task<IEnumerable<CollectionCardDto>> Handle(GetCollectionCardsRequest request, CancellationToken cancellationToken)
    {
        var currentUser = await _userService.GetCurrentUser(cancellationToken);
        
        _userValidationService.ValidateUserId(currentUser?.Id);

        return await _collectionRepository.GetCollectionCardsAsync(currentUser!.Id, request.ParentCollectionId, cancellationToken);
    }
}