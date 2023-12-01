using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace E_Lang.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BaseController<TEntity, TDto> : ControllerBase
    where TEntity : EntityBase
    where TDto: IMapper<TEntity>
{
    protected readonly IMediator _mediator;
    private readonly IRepository<TEntity, TDto> _repository;

    public BaseController(IMediator mediator, IRepository<TEntity, TDto> repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    [HttpGet()]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        var entities = await _repository.GetAllAsDtoAsync(default);
        return Ok(entities);
    }
}