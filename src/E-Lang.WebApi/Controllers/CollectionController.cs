using E_Lang.Application.Collections.Requests;
using E_Lang.Application.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Lang.WebApi.Controllers;

public class CollectionController : BaseController
{
    public CollectionController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("collection-cards")]
    public async Task<IEnumerable<CollectionCardDto>> GetCollectionCards(Guid? parentCollectionId)
    {
        return await _mediator.Send(new GetCollectionCardsRequest() { ParentCollectionId = parentCollectionId });
    }

    [HttpGet("collection")]
    public async Task<CollectionDto> GetCollection(Guid? collectionId)
    {
        return await _mediator.Send(new GetCollectionRequest() {CollectionId = collectionId});
    }
}