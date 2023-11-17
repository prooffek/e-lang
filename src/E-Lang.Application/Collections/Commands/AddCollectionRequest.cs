using System.Data;
using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace E_Lang.Application.Collections.Commands;

public record AddCollectionRequest : IRequest<CollectionDto>
{
    public CreateCollectionDto CollectionDto { get; set; }
}

public class AddCollectionRequestHandler : IRequestHandler<AddCollectionRequest, CollectionDto>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public AddCollectionRequestHandler(ICollectionRepository collectionRepository, IMapper mapper, 
        IUserService userService)
    {
        _collectionRepository = collectionRepository;
        _mapper = mapper;
        _userService = userService;
    }
    
    public async Task<CollectionDto> Handle(AddCollectionRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new DataException("User not found.");
        
        var newCollection = _mapper.Map<Collection>(request.CollectionDto);
        newCollection.OwnerId = user.Id;
        
        _collectionRepository.Add(newCollection);
        await _collectionRepository.SaveAsync(cancellationToken);

        return await _collectionRepository.GetCollectionAsDtoAsync(newCollection.Id, cancellationToken)
            ?? throw new ArgumentException($"New collection ${newCollection.Name} not found.");
    }
}