using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public class MeaningBuilder<TParentBuilder> : EntityBuilderBase<Meaning, TParentBuilder>
        where TParentBuilder : class
    {
        public MeaningBuilder(Meaning entity, TParentBuilder parentBuilder, IAppDbContext context) : base(entity, parentBuilder, context)
        {
        }

        public MeaningBuilder<TParentBuilder> SetValue(string value)
        {
            _entity.Value = value;
            return this;
        }

        public MeaningBuilder<TParentBuilder> SetFlashcardId(Guid flashcardId)
        {
            _entity.FlashcardId = flashcardId;
            return this;
        }
    }
}
