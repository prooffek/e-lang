using System.Linq.Expressions;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public abstract class Repository<TEntity, TDto> : IRepository<TEntity, TDto> 
    where TEntity : EntityBase
    where TDto : IMapper<TEntity>
{
    protected readonly IAppDbContext _dbContext;
    protected readonly DbSet<TEntity> _entity;
    
    protected IQueryable<TEntity> _queryWithIncludes => GetQueryWithIncludes();

    public Repository(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
        _entity = _dbContext.GetDbSet<TEntity>();
    }
    
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _entity
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public abstract Task<TDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entity.ToListAsync(cancellationToken);
    }

    public abstract Task<IEnumerable<TDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default);

    public virtual void Add(TEntity entity)
    {
        _entity.Add(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _entity.AddRange(entities);
    }

    public virtual void Update(TEntity entity)
    {
        _entity.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _entity.UpdateRange(entities);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _entity.AnyAsync(expression, cancellationToken);
    }

    public virtual async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    protected virtual IQueryable<TEntity> GetQueryWithIncludes()
    {
            return _entity.AsQueryable();
    }
}