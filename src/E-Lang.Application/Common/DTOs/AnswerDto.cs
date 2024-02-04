using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class AnswerDto : IMapper<Meaning>
{
    public string Value { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<Meaning, AnswerDto>()
            .Map(dest => dest.Value, src => src.Value);
    }
}