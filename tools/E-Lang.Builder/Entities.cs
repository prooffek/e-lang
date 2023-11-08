using E_Lang.Domain.Entities;

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
    }
}
