using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using MediatR;

namespace E_Lang.Application.Collections.Requests;

public record GetCollectionAutocompleteDataRequest : IRequest<IEnumerable<CollectionAutocompleteDto>>
{
}

public class GetCollectionAutocompleteDataRequestHandler : IRequestHandler<GetCollectionAutocompleteDataRequest,
    IEnumerable<CollectionAutocompleteDto>>
{
    private readonly IUserService _userService;
    private readonly ICollectionRepository _collectionRepository;

    public GetCollectionAutocompleteDataRequestHandler(IUserService userService, ICollectionRepository collectionRepository)
    {
        _userService = userService;
        _collectionRepository = collectionRepository;
    }
    
    public async Task<IEnumerable<CollectionAutocompleteDto>> Handle(GetCollectionAutocompleteDataRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();
        
        return await _collectionRepository.GetAutocompleteData(user.Id, cancellationToken);
    }
}