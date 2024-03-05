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
            if (_entity.AttemptQuizTypes is null)
            {
                _entity.AttemptQuizTypes = new List<AttemptQuizType>();
            }

            var attemptQuizType = new AttemptQuizType
            {
                AttemptId = _entity.Id,
                QuizTypeId = firstQuiz.Id,
                QuizType = firstQuiz
            };

            _entity.AttemptQuizTypes.Add(attemptQuizType);
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
            _entity.AttemptStages ??= new List<AttemptStage>();
            _entity.AttemptStages.Add(attemptStage);
            return new AttemptStageBuilder<AttemptBuilder<TParentBuilder>>(attemptStage, this, _context, _dateTimeProvider);
        }

        public AttemptBuilder<TParentBuilder> SetCompletedFlashcards(ICollection<Flashcard> flashcards)
        {
            AttemptStage? stage = _entity.AttemptStages.FirstOrDefault(x => x.Stage == AttemptStageType.Complete);

            if (stage == null)
            {
                stage = Entities.GetAttemptStage();
                _entity.AttemptStages.Add(stage);
            }

            foreach (var flashcard in flashcards)
            {
                var flashcardState = Entities.GetCompletedFlashcardState(flashcard);
                stage.Flashcards.Add(flashcardState);
            }
            
            return this;
        }

        public FlashcardBuilder<AttemptBuilder<TParentBuilder>> AddCompletedFlashcard(out Flashcard flashcard)
        {
            if (_entity.Collection is null)
                throw new NullReferenceException("AttemptBuilder: Collection not assigned to the attempt.");

            flashcard = Entities.GetFlashcard(_entity.CollectionId, _entity.Collection.OwnerId);

            AttemptStage? stage = _entity.AttemptStages.FirstOrDefault(x => x.Stage == AttemptStageType.Complete);

            if (stage == null)
            {
                stage = Entities.GetAttemptStage();
                stage.Stage = AttemptStageType.Complete;
                _entity.AttemptStages.Add(stage);
            }

            var flashcardState = Entities.GetCompletedFlashcardState(flashcard);
            stage.Flashcards.Add(flashcardState);
            return new FlashcardBuilder<AttemptBuilder<TParentBuilder>>(flashcard, this, _context, _dateTimeProvider);
        }

        public AttemptBuilder<TParentBuilder> AddQuizType(QuizType quizType)
        {
            if (_entity.QuizTypes is null)
            {
                _entity.AttemptQuizTypes = new List<AttemptQuizType>();
            }

            var attemptQuizType = new AttemptQuizType
            {
                AttemptId = _entity.Id,
                QuizTypeId = quizType.Id,
                QuizType = quizType
            };

            _entity.AttemptQuizTypes.Add(attemptQuizType);
            return this;
        }

        public AttemptBuilder<TParentBuilder> AddQuizTypes(IEnumerable<QuizType> quizTypes)
        {
            if (_entity.QuizTypes is null)
            {
                _entity.AttemptQuizTypes = new List<AttemptQuizType>();
            }

            foreach (var quizType in quizTypes)
            {
                if (!_entity.QuizTypes.Any(q => q.Id == quizType.Id))
                {
                    var attemptQuizType = new AttemptQuizType
                    {
                        AttemptId = _entity.Id,
                        QuizTypeId = quizType.Id,
                        QuizType = quizType
                    };

                    _entity.AttemptQuizTypes.Add(attemptQuizType);
                }
            }

            return this;
        }

        public AttemptStageBuilder<AttemptBuilder<TParentBuilder>> AddAttemptStage(out AttemptStage attemptStage)
        {
            attemptStage = Entities.GetAttemptStage();

            _entity.AttemptStages ??= new List<AttemptStage>();

            _entity.AttemptStages.Add(attemptStage);

            return new AttemptStageBuilder<AttemptBuilder<TParentBuilder>>(attemptStage, this, _context, _dateTimeProvider);
        }
    }
}
