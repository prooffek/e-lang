using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class CollectionDto : IMapper<Collection>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public IEnumerable<CollectionCardDto>? Subcollections { get; set; }
    public IEnumerable<FlashcardDto>? Flashcards { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<Collection, CollectionDto>()
            .Map(dest => dest.ParentName, src => src.Parent != null ? src.Parent.Name : null);
    }
}