using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders;

public class CollectionBuilder<TParentBuilder> : EntityBuilderBase<Collection, TParentBuilder>
    where TParentBuilder : class
{
    public CollectionBuilder(Collection entity, TParentBuilder parentBuilder, IAppDbContext context, IDateTimeProvider dateTimeProvider) 
        : base(entity, parentBuilder, context, dateTimeProvider)
    {
    }

    public CollectionBuilder<TParentBuilder> SetName(string collectionName)
    {
        _entity.Name = collectionName;
        return this;
    }

    public CollectionBuilder<TParentBuilder> SetParentCollection(Collection parentCollection)
    {
        _entity.ParentId = parentCollection.Id;
        _entity.Parent = parentCollection;
        return this;
    }

    public CollectionBuilder<TParentBuilder> SetOwner(Guid userId)
    {
        _entity.OwnerId = userId;
        return this;
    }

    public CollectionBuilder<CollectionBuilder<TParentBuilder>> AddSubcollection(out Collection subcollection)
    {
        subcollection = Entities.GetCollection(_entity.OwnerId);
        subcollection.ParentId = _entity.Id;
        subcollection.Parent = _entity;
        return new CollectionBuilder<CollectionBuilder<TParentBuilder>>(subcollection, this, _context, _dateTimeProvider);
    }

    public FlashcardBuilder<CollectionBuilder<TParentBuilder>> AddFlashcard(out Flashcard flashcard)
    {
        flashcard = Entities.GetFlashcard(_entity.Id, _entity.OwnerId);
        _entity.Flashcards.Add(flashcard);
        return new FlashcardBuilder<CollectionBuilder<TParentBuilder>>(flashcard, this, _context, _dateTimeProvider);
    }

    public AttemptBuilder<CollectionBuilder<TParentBuilder>> AddAttempt(out Attempt attempt, QuizType firstQuiz)
    {
        attempt = Entities.GetAttempt(_entity);
        if(_entity.Attempts is null)
            _entity.Attempts = new List<Attempt>();

        _entity.Attempts.Add(attempt);

        return new AttemptBuilder<CollectionBuilder<TParentBuilder>>(attempt, firstQuiz, this, _context, _dateTimeProvider);
    }
}