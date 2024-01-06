using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs
{
    public class FlashcardStateDto : IMapper<FlashcardState>, IMapper<InitFlashcardState>, IMapper<InProgressFlashcardState>
    {
        public FlashcardDto? Flashcard { get; set; }

        public void Map(TypeAdapterConfig config)
        {
            config.NewConfig<FlashcardState, FlashcardStateDto>();
        }
    }
}
