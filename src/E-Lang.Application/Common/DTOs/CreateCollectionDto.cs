using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class CreateCollectionDto : IMapper<Collection>
{
    public string Name { get; set; }
    public Guid? ParentCollectionId { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<CreateCollectionDto, Collection>()
            .Map(dest => dest.ParentId, src => src.ParentCollectionId)
            .Map(dest => dest.Name, src => src.Name);
    }
}