using E_Lang.Application.Interfaces;
using E_Lang.Builder.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public abstract class BuilderBase<T, TParentBuilder> : IBuilder<TParentBuilder> 
        where T: class 
        where TParentBuilder : class
    {
        protected readonly T _entity;
        protected readonly TParentBuilder _parentBuilder;
        protected readonly IDateTimeProvider _dateTimeProvider;

        public BuilderBase(T entity, TParentBuilder parentBuilder, IDateTimeProvider dateTimeProvider)
        {
            _entity = entity;
            _parentBuilder = parentBuilder;
            _dateTimeProvider = dateTimeProvider;
        }

        public virtual TParentBuilder Build()
        {
            if (_entity is EntityBase entity)
            {
                entity.CreatedOn = _dateTimeProvider.UtcNow;
                entity.ModifiedOn = _dateTimeProvider.UtcNow;
            }
            
            
            return _parentBuilder;
        }
    }
}
