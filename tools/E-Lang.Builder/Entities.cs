using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Builder
{
    public static class Entities
    {
        public static User GetUser()
        {
            return new User()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                UserName = "Admin",
                Email = "test@test.com",
            };
        }
        
        public static Collection GetCollection(Guid userId)
        {
            return new Collection()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Name = "Test collection",
                ParentId = null,
                OwnerId = userId,
                Subcollections = new List<Collection>(),
                Flashcards = new List<Flashcard>()
            };
        }

        public static Meaning GetMeaning()
        {
            return new Meaning()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Value = "Test vaue",
                FlashcardBases = new List<FlashcardBase>()
            };
        }

        public static FlashcardBase GetFlashcardBase()
        {
            return new FlashcardBase()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                WordOrPhrase = "Test",
                Meanings = new List<Meaning>()
            };
        }

        public static Flashcard GetFlashcard(Guid collectionId, Guid ownerId)
        {
            return new Flashcard()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                CollectionId = collectionId,
                OwnerId = ownerId,
                Status = FlashcardStatus.Active,
                LastStatusChangedOn = DateTime.UtcNow,
                LastSeenOn = DateTime.UtcNow,
            };
        }
    }
}
