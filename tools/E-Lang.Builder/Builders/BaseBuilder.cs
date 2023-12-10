using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Interfaces;
using E_Lang.Builder.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public class BaseBuilder : IBaseBuilder
    {
        private readonly IAppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public BaseBuilder(IAppDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public UserBuilder AddUser(out User user)
        {
            user = Entities.GetUser();
            return new UserBuilder(user, this, _context, _dateTimeProvider);
        }

        public CollectionBuilder<BaseBuilder> AddCollection(out Collection collection, Guid userId)
        {
            collection = Entities.GetCollection(userId);
            return new CollectionBuilder<BaseBuilder>(collection, this, _context, _dateTimeProvider);
        }

        public FlashcardBuilder<BaseBuilder> AddFlashcard(out Flashcard flashcard, Guid collectionId, Guid userId)
        {
            flashcard = Entities.GetFlashcard(collectionId, userId);
            return new FlashcardBuilder<BaseBuilder>(flashcard, this, _context, _dateTimeProvider);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
