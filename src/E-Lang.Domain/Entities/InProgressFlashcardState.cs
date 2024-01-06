namespace E_Lang.Domain.Entities
{
    public class InProgressFlashcardState : FlashcardState
    {
        public Guid? CurrentQuizTypeId { get; set; }
        public QuizType? CurrentQuizType { get; set; }

        public DateTime? ShowAgainOn { get; set; }

        public ICollection<QuizType>? CompletedQuizTypes { get; set; }

        public InProgressFlashcardState()
        {
            CompletedQuizTypes = new List<QuizType>();
        }
    }
}
