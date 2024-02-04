using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Persistence.Repositories;

public abstract class RepositoryWithDto<TEntity, TDto> : Repository<TEntity>, IRepositoryWithDto<TEntity, TDto>
    where TEntity : EntityBase
    where TDto : IMapper<TEntity>
{
    protected RepositoryWithDto(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
    {
    }

    public abstract Task<TDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<TDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default);
}