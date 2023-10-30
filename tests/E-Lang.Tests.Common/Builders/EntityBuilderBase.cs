using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Tests.Common.Builders
{
    public abstract class EntityBuilderBase<TEntity, TParentBuilder> : BuilderBase<TEntity, TParentBuilder>
        where TEntity : EntityBase, new()
        where TParentBuilder: class
    {
        protected readonly IAppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EntityBuilderBase(TEntity entity, TParentBuilder parentBuilder, IAppDbContext context)
            : base(entity, parentBuilder)
        {
            _context = context;
            _dbSet = _context.GetDbSet<TEntity>();
        }

        public override TParentBuilder Build()
        {
            _dbSet.Add(_entity);
            return base.Build();
        }
    }
}
