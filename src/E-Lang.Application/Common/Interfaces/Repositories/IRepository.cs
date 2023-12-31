using System.Linq.Expressions;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface IRepository<TEntity> 
    where TEntity : EntityBase 
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}