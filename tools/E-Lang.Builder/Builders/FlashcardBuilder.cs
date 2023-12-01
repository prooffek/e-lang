using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Builder.Builders
{
    public class FlashcardBuilder<TParentBuilder> : EntityBuilderBase<Flashcard, TParentBuilder>
        where TParentBuilder : class
    {
        public FlashcardBuilder(Flashcard entity, TParentBuilder parentBuilder, IAppDbContext context) : base(entity, parentBuilder, context)
        {
        }

        public FlashcardBaseBuilder<TParentBuilder> AddFlashcardBase(out FlashcardBase flashcardBase)
        {
            flashcardBase = Entities.GetFlashcardBase();
            _entity.FlashcardBaseId = flashcardBase.Id;
            return new FlashcardBaseBuilder<TParentBuilder>(flashcardBase, this, _context);
        }

        public MeaningBuilder<FlashcardBuilder<TParentBuilder>> AddMeaning(out Meaning meaning)
        {
            meaning = Entities.GetMeaning();
            meaning.FlashcardId = _entity.Id;
            return new MeaningBuilder<FlashcardBuilder<TParentBuilder>>(meaning, this, _context);
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
