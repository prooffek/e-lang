using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class CollectionAutocompleteDto : IMapper<Collection>
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<Collection, CollectionAutocompleteDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
    }
}