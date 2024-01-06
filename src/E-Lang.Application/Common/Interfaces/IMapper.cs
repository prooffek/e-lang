using Mapster;

namespace E_Lang.Application.Common.Interfaces;

public interface IMapper<TEntity> where TEntity : class
{
    void Map(TypeAdapterConfig config) 
    {
        config.NewConfig(typeof(TEntity), GetType());
        config.NewConfig(GetType(), GetType());
        config.NewConfig(GetType(), typeof(TEntity));
    }
}