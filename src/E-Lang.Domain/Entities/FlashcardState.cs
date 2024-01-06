namespace E_Lang.Domain.Entities
{
    public abstract class FlashcardState : EntityBase
    {
        public Guid? FlashcardId { get; set; }
        public Flashcard? Flashcard { get; set; }
    }
}
