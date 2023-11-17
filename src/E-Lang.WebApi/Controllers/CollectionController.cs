using E_Lang.Application.Collections.Commands;
using E_Lang.Application.Collections.Requests;
using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Collection = E_Lang.Domain.Entities.Collection;

namespace E_Lang.WebApi.Controllers;

public class CollectionController : BaseController<Collection, CollectionCardDto>
{
    public CollectionController(IMediator mediator, ICollectionRepository repository) 
        : base(mediator, repository)
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

    [HttpPost("add-collection")]
    public async Task<ActionResult<CollectionDto>> AddCollection([FromBody] CreateCollectionDto collectionDto)
    {
        return await _mediator.Send(new AddCollectionRequest() {CollectionDto = collectionDto});
    }
    
    [HttpPut("update-collection")]
    public async Task<CollectionDto?> UpdateCollection([FromBody] UpdateCollectionDto collectionDto)
    {
        return await _mediator.Send(new UpdateCollectionRequest() { UpdateDto = collectionDto});
    }
    
    [HttpDelete("delete-collection")]
    public async Task<IActionResult> DeleteCollection(Guid? collectionId)
    {
        await _mediator.Send(new DeleteCollectionRequest() { Id = collectionId });
        return Ok();
    }

    [HttpGet("collection-autocomplete-data")]
    public async Task<IEnumerable<CollectionAutocompleteDto>> GetCollectionAutocompleteData()
    {
        return await _mediator.Send(new GetCollectionAutocompleteDataRequest());
    }
}