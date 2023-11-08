using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class CollectionCardDto : IMapper<Collection>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int SubcollectionsCount { get; set; }
    public int  FlashcardsCount { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<Collection, CollectionCardDto>()
            .Map(dest => dest.Title, src => src.Name)
            .Map(dest => dest.SubcollectionsCount, src => src.Subcollections != null ? src.Subcollections.Count() : 0)
            .Map(dest => dest.FlashcardsCount, src => src.Flashcards != null ? src.Flashcards.Count() : 0);
    }
}