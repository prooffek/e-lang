using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
{
    private readonly IAppDbContext _dbContext;
    protected readonly DbSet<TEntity> _entity;

    public Repository(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
        _entity = dbContext.GetDbSet<TEntity>();
    }
    
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _entity
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
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

    public virtual async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}