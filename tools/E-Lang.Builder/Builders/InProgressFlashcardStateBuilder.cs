using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public class InProgressFlashcardStateBuilder<TParentBuilder> : EntityBuilderBase<InProgressFlashcardState, TParentBuilder>
        where TParentBuilder : class
    {
        public InProgressFlashcardStateBuilder(InProgressFlashcardState entity, TParentBuilder parentBuilder, IAppDbContext context, IDateTimeProvider dateTimeProvider) 
            : base(entity, parentBuilder, context, dateTimeProvider)
        {
        }

        public FlashcardBuilder<InProgressFlashcardStateBuilder<TParentBuilder>> AddFlashcard(out Flashcard flashcard, Guid collectionId, Guid userId)
        {
            flashcard = Entities.GetFlashcard(collectionId, userId);
            _entity.Flashcard = flashcard;

            return new FlashcardBuilder<InProgressFlashcardStateBuilder<TParentBuilder>>(flashcard, this, _context, _dateTimeProvider);
        }

        public InProgressFlashcardStateBuilder<TParentBuilder> SetShowAgainOn(DateTime seeAgainOn)
        {
            _entity.ShowAgainOn = seeAgainOn;
            return this;
        }

        public InProgressFlashcardStateBuilder<TParentBuilder> AddSeenQuizType(QuizType quizType)
        {
            AddSeenQuizType(quizType.Id);

            return this;
        }

        public InProgressFlashcardStateBuilder<TParentBuilder> AddSeenQuizTypes(IEnumerable<QuizType> quizTypes)
        {
            foreach (var quizType in quizTypes)
            {
                AddSeenQuizType(quizType.Id);
            } 

            return this;
        }

        public InProgressFlashcardStateBuilder<TParentBuilder> AddCompletedQuizType(QuizType quizType)
        {
            AddSeenQuizType(quizType.Id);
            AddCompletedQuizType(quizType.Id);

            return this;
        }

        public InProgressFlashcardStateBuilder<TParentBuilder> AddCompletedQuizTypes(IEnumerable<QuizType> quizTypes)
        {
            foreach (var quizType in quizTypes)
            {
                AddSeenQuizType(quizType.Id);
                AddCompletedQuizType(quizType.Id);
            }

            return this;
        }

        private void AddSeenQuizType(Guid quizTypeId)
        {
            if (!_entity.SeenQuizTypes.Any(q => q.QuizTypeId == quizTypeId))
                _entity.SeenQuizTypes.Add(new SeenQuizType(_entity.Id, quizTypeId));
        }

        private void AddCompletedQuizType(Guid quizTypeId)
        {
            if (!_entity.CompletedQuizTypes.Any(q => q.QuizTypeId == quizTypeId))
                _entity.CompletedQuizTypes.Add(new CompletedQuizType(_entity.Id, quizTypeId));
        }
    }
}
