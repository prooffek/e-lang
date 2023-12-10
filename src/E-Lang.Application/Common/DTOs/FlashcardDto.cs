using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class FlashcardDto : IMapper<Flashcard>
{
    public Guid Id { get; set; }
    public string CollectionName { get; set; }
    public string WordOrPhrase { get; set; }
    public ICollection<MeaningDto> Meanings { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastSeenOn { get; set; }
    public FlashcardStatus Status { get; set; }
    public Guid FlashcardBaseId { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<Flashcard, FlashcardDto>()
            .Map(dest => dest.CollectionName, src => src.Collection.Name)
            .Map(dest => dest.WordOrPhrase, src => src.FlashcardBase.WordOrPhrase)
            .Map(dest => dest.Meanings, src => src.FlashcardBase.Meanings);
    }
}