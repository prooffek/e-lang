using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public record AddOrUpdateMeaningDto : IMapper<Meaning>
{
    public Guid? Id { get; set; }
    public string Value { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<AddOrUpdateMeaningDto, Meaning>()
            .Map(src => src.Value, dest => dest.Value);
    }
}