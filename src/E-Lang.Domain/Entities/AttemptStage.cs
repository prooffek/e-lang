namespace E_Lang.Domain.Entities
{
    public abstract class AttemptStage : EntityBase
    {
        public ICollection<FlashcardState>? Flashcards { get; set; }
    }
}
