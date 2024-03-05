using E_Lang.Domain.Enums;
using System.ComponentModel;

namespace E_Lang.Domain.Entities
{
    public class AttemptStage : EntityBase
    {
        [DefaultValue(AttemptStageType.Init)]
        public AttemptStageType Stage { get; set; }

        public ICollection<FlashcardState>? Flashcards { get; set; }

        public Guid AttemptId { get; set; }
    }
}
