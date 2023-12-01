using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public class FlashcardBaseBuilder<TParentBuilder> : EntityBuilderBase<FlashcardBase, FlashcardBuilder<TParentBuilder>>
        where TParentBuilder : class
    {
        public FlashcardBaseBuilder(FlashcardBase entity, FlashcardBuilder<TParentBuilder> parentBuilder, IAppDbContext context) : base(entity, parentBuilder, context)
        {
        }

        public FlashcardBaseBuilder<TParentBuilder> SetWordOrPhrase(string wordOrPhrase)
        {
            _entity.WordOrPhrase = wordOrPhrase;
            return this;
        }

        public  MeaningBuilder<FlashcardBaseBuilder<TParentBuilder>> AddMeaning(out Meaning meaning, Guid flashcardId)
        {
            meaning = Entities.GetMeaning();
            meaning.FlashcardId = flashcardId;
            _entity.Meanings.Add(meaning);
            return new MeaningBuilder<FlashcardBaseBuilder<TParentBuilder>>(meaning, this, _context);
        }

        public FlashcardBaseBuilder<TParentBuilder> AddMeaning(Meaning meaning)
        {
            _entity.Meanings.Add(meaning);
            return this;
        }
    }
}
