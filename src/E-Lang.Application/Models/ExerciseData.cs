using E_Lang.Domain.Entities;

namespace E_Lang.Application.Models
{
    public struct ExerciseData
    {
        public Guid AttemptId { get; }
        public Guid FlashcardStateId { get; }
        public Guid CollectionId { get; }
        public FlashcardBase FlashcardBase { get; }
        public QuizType QuizType { get; }

        public ExerciseData(Guid attemptId, Guid flashcardStateId, Guid collectionId, FlashcardBase flashcardBase, QuizType quizType)
        {
            AttemptId = attemptId;
            FlashcardStateId = flashcardStateId;
            CollectionId = collectionId;
            FlashcardBase = flashcardBase;
            QuizType = quizType;
        }
    }
}
