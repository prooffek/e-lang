using E_Lang.Application.Common.DTOs;
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
    private readonly IUserValidationService _userValidationService;
    private readonly ICollectionRepository _collectionRepository;

    public GetCollectionRequestHandler(IUserService userService, ICollectionService collectionService,
        IUserValidationService userValidationService, ICollectionRepository collectionRepository)
    {
        _userService = userService;
        _collectionService = collectionService;
        _userValidationService = userValidationService;
        _collectionRepository = collectionRepository;
    }
    
    public async Task<CollectionDto> Handle(GetCollectionRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken);
        _userValidationService.ValidateUserId(user?.Id);
        await _collectionService.ValidateUserCollectionId(user!.Id, request.CollectionId, cancellationToken);

        return await _collectionRepository.GetCollectionAsDtoAsync(request.CollectionId.Value, cancellationToken);
    }
}