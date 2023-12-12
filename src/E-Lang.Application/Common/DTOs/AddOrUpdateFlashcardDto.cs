using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public record AddOrUpdateFlashcardDto : IMapper<Flashcard>
{
    public Guid? FlashcardId { get; set; }
    public Guid CollectionId { get; set; }
    public Guid? FlashcardBaseId { get; set; }
    public string WordOrPhrase { get; set; }
    public ICollection<AddOrUpdateMeaningDto> Meanings { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<AddOrUpdateFlashcardDto, Flashcard>()
            .Map(dest => dest.Id, src => src.FlashcardId)
            .Map(dest => dest.CollectionId, src => src.CollectionId)
            .Map(dest => dest.Status, src => FlashcardStatus.Active)
            .Map(dest => dest.FlashcardBaseId, src => src.FlashcardBaseId)
            .Map(dest => dest.FlashcardBase, src => src);
        
        config.NewConfig<AddOrUpdateFlashcardDto, FlashcardBase>()
            .Map(dest => dest.Id, src => src.FlashcardBaseId)
            .Map(dest => dest.WordOrPhrase, src => src.WordOrPhrase)
            .Map(dest => dest.Meanings, src => src.Meanings);
    }
}