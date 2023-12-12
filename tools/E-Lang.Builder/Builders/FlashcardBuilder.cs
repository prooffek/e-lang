using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Builder.Builders
{
    public class FlashcardBuilder<TParentBuilder> : EntityBuilderBase<Flashcard, TParentBuilder>
        where TParentBuilder : class
    {
        public FlashcardBuilder(Flashcard entity, TParentBuilder parentBuilder, IAppDbContext context
            , IDateTimeProvider dateTimeProvider) : base(entity, parentBuilder, context, dateTimeProvider)
        {
        }

        public FlashcardBaseBuilder<TParentBuilder> AddFlashcardBase(out FlashcardBase flashcardBase)
        {
            flashcardBase = Entities.GetFlashcardBase();
            _entity.FlashcardBaseId = flashcardBase.Id;
            return new FlashcardBaseBuilder<TParentBuilder>(flashcardBase, this, _context, _dateTimeProvider);
        }

        public MeaningBuilder<FlashcardBuilder<TParentBuilder>> AddMeaning(out Meaning meaning, Guid flashcardbaseId)
        {
            meaning = Entities.GetMeaning(flashcardbaseId);
            return new MeaningBuilder<FlashcardBuilder<TParentBuilder>>(meaning, this, _context, _dateTimeProvider);
        }

        public FlashcardBuilder<TParentBuilder> SetFlashcardBase(FlashcardBase flashcardBase)
        {
            _entity.FlashcardBaseId = flashcardBase.Id;
            _entity.FlashcardBase = flashcardBase;
            return this;
        }

        public FlashcardBuilder<TParentBuilder> SetOwner(Guid ownerId)
        {
            _entity.OwnerId = ownerId;
            return this;
        }

        public FlashcardBuilder<TParentBuilder> SetStatus(FlashcardStatus status)
        {
            _entity.Status = status;
            return this;
        }

        public FlashcardBuilder<TParentBuilder> SetLastStatusChangedOn(DateTime date)
        {
            _entity.LastStatusChangedOn = date;
            return this;
        }
        
        public FlashcardBuilder<TParentBuilder> SetLastSeenOn(DateTime date)
        {
            _entity.LastSeenOn = date;
            return this;
        }
    }
}
