using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Builder.Builders
{
    public class AttemptBuilder<TParentBuilder> : EntityBuilderBase<Attempt, TParentBuilder>
        where TParentBuilder : class
    {
        public AttemptBuilder(Attempt entity, QuizType firstQuiz, TParentBuilder parentBuilder, IAppDbContext context, IDateTimeProvider dateTimeProvider) 
            : base(entity, parentBuilder, context, dateTimeProvider)
        {
            if (_entity.QuizTypes is null)
            {
                _entity.QuizTypes = new List<QuizType>();
            }

            _entity.QuizTypes.Add(firstQuiz);
        }

        public AttemptBuilder<TParentBuilder> SetName(string name)
        {
            _entity.Name = name;
            return this;
        }

        public AttemptBuilder<TParentBuilder> SetMaxFlashcardsPerStage(int maxFlashcardsPerStage)
        {
            _entity.MaxFlashcardsPerStage = maxFlashcardsPerStage;
            return this;
        }

        public AttemptBuilder<TParentBuilder> SetMaxQuizTypesPerFlashcard(int maxQuizTypesPerFlashcard)
        {
            _entity.MaxQuizTypesPerFlashcard = maxQuizTypesPerFlashcard;
            return this;
        }

        public AttemptBuilder<TParentBuilder> SetMinCompletedQuizzesPerCent(int minCompletedQuizzesPerCent)
        {
            _entity.MinCompletedQuizzesPerCent = minCompletedQuizzesPerCent;
            return this;
        }

        public AttemptBuilder<TParentBuilder> SetOrder(FlashcardOrder order)
        {
            _entity.Order = order;
            return this;
        }

        public AttemptBuilder<TParentBuilder> SetIncludeMeanings(bool includeMeanings)
        {
            _entity.IncludeMeanings = includeMeanings;
            return this;
        }

        public AttemptStageBuilder<AttemptBuilder<TParentBuilder>> AddInitAttemptStageAsCurrentStage(out AttemptStage attemptStage)
        {
            attemptStage = Entities.GetAttemptStage();
            _entity.CurrentStage = attemptStage;
            return new AttemptStageBuilder<AttemptBuilder<TParentBuilder>>(attemptStage, this, _context, _dateTimeProvider);
        }

        public AttemptBuilder<TParentBuilder> SetCompletedFlashcards(ICollection<Flashcard> flashcards)
        {
            _entity.CompletedFlashcards = flashcards;
            return this;
        }

        public FlashcardBuilder<AttemptBuilder<TParentBuilder>> AddCompletedFlashcard(out Flashcard flashcard)
        {
            if (_entity.Collection is null)
                throw new NullReferenceException("AttemptBuilder: Collection not assigned to the attempt.");

            flashcard = Entities.GetFlashcard(_entity.CollectionId, _entity.Collection.OwnerId);

            if (_entity.CompletedFlashcards is null)
                _entity.CompletedFlashcards = new List<Flashcard>();

            _entity.CompletedFlashcards.Add(flashcard);
            return new FlashcardBuilder<AttemptBuilder<TParentBuilder>>(flashcard, this, _context, _dateTimeProvider);
        }

        public AttemptBuilder<TParentBuilder> AddQuizType(QuizType quizType)
        {
            if (_entity.QuizTypes is null)
            {
                _entity.QuizTypes = new List<QuizType>();
            }
            
            _entity.QuizTypes.Add(quizType);
            return this;
        }

        public AttemptBuilder<TParentBuilder> AddQuizTypes(IEnumerable<QuizType> quizTypes)
        {
            if (_entity.QuizTypes is null)
            {
                _entity.QuizTypes = new List<QuizType>();
            }

            foreach (var quizType in quizTypes)
            {
                if (!_entity.QuizTypes.Any(q => q.Id == quizType.Id))
                    _entity.QuizTypes.Add(quizType);
            }

            return this;
        }
    }
}
