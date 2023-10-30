using E_Lang.Tests.Common.Interfaces;

namespace E_Lang.Tests.Common.Builders
{
    public abstract class BuilderBase<T, TParentBuilder> : IBuilder<TParentBuilder> 
        where T: class 
        where TParentBuilder : class
    {
        protected readonly T _entity;
        protected readonly TParentBuilder _parentBuilder;

        public BuilderBase(T entity, TParentBuilder parentBuilder)
        {
            _entity = entity;
            _parentBuilder = parentBuilder;
        }

        public virtual TParentBuilder Build()
        {
            return _parentBuilder;
        }
    }
}
