using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.Common.DTOs
{
    public class AttemptStageDto : IMapper<AttemptStage>
    {
        public Guid Id { get; set; }
        public AttemptStageType Stage { get; set; }
        public ICollection<FlashcardStateDto>? Flashcards { get; set; }
    }
}
