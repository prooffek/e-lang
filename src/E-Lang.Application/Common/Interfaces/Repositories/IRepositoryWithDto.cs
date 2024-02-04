using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface IRepositoryWithDto<TEntity, TDto> where TEntity : EntityBase where TDto : IMapper<TEntity>
{
    Task<TDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default);
}