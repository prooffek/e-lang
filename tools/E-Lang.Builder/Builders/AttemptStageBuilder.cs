using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Builder.Builders
{
    public class AttemptStageBuilder<TParentBuilder> : EntityBuilderBase<AttemptStage, TParentBuilder>
        where TParentBuilder : class
    {
        public AttemptStageBuilder(AttemptStage entity, TParentBuilder parentBuilder, IAppDbContext context, IDateTimeProvider dateTimeProvider) 
            : base(entity, parentBuilder, context, dateTimeProvider)
        {
        }

        public AttemptStageBuilder<TParentBuilder> SetStage(AttemptStageType stage)
        {
            _entity.Stage = stage;
            return this;
        }

        public AttemptStageBuilder<TParentBuilder> SetFlashcardStates(ICollection<FlashcardState> flashcardStates)
        {
            _entity.Flashcards = flashcardStates;
            return this;
        }

        public FlashcardBuilder<AttemptStageBuilder<TParentBuilder>> AddFlashcard(out Flashcard flashcard, Collection collection)
        {
            flashcard = Entities.GetFlashcard(collection.Id, collection.OwnerId);

            if (_entity.Flashcards is null)
                _entity.Flashcards = new List<FlashcardState>();

            _entity.Flashcards.Add(Entities.GetInitFlashcardState(flashcard));

            return new FlashcardBuilder<AttemptStageBuilder<TParentBuilder>>(flashcard, this, _context, _dateTimeProvider);
        }
    }
}
