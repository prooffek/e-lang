using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using MediatR;

namespace E_Lang.Application.Collections.Requests;

public record GetCollectionRequest : IRequest<CollectionDto>
{
    public Guid? CollectionId { get; set; }
}

public class GetCollectionRequestHandler : IRequestHandler<GetCollectionRequest, CollectionDto>
{
    private readonly IUserService _userService;
    private readonly ICollectionService _collectionService;
    private readonly ICollectionRepository _collectionRepository;

    public GetCollectionRequestHandler(IUserService userService, ICollectionService collectionService,
        ICollectionRepository collectionRepository)
    {
        _userService = userService;
        _collectionService = collectionService;
        _collectionRepository = collectionRepository;
    }
    
    public async Task<CollectionDto> Handle(GetCollectionRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();

        await _collectionService.ValidateUserCollectionId(user!.Id, request.CollectionId, ActionTypes.Get, cancellationToken);

        return await _collectionRepository.GetCollectionAsDtoAsync(request.CollectionId.Value, cancellationToken);
    }
}