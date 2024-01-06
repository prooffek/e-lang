using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs
{
    public class AttemptStageDto : IMapper<AttemptStage>
    {
        public Guid Id { get; set; }
        public ICollection<FlashcardStateDto>? Flashcards { get; set; }
    }
}
