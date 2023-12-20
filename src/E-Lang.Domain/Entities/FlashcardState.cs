namespace E_Lang.Domain.Entities
{
    public abstract class FlashcardState : EntityBase
    {
        public Guid? CurrentQuizTypeId { get; set; }
        public QuizType? CurrentQuizType { get; set; }
        
        public Guid? FlashcardId { get; set; }
        public Flashcard? Flashcard { get; set; }
        
        public DateTime? ShowAgainOn { get; set; }
        
        public ICollection<QuizType>? CompletedQuizTypes { get; set; }
    }
}
