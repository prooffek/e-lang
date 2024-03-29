using System.Linq.Expressions;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : EntityBase
{
    protected readonly IAppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    protected readonly DbSet<TEntity> _entity;
    
    protected IQueryable<TEntity> _queryWithIncludes => GetQueryWithIncludes();

    public Repository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _entity = _dbContext.GetDbSet<TEntity>();
    }
    
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _queryWithIncludes
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entity.ToListAsync(cancellationToken);
    }

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

    public virtual void Delete(TEntity entity)
    {
        _entity.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _entity.RemoveRange(entities);
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