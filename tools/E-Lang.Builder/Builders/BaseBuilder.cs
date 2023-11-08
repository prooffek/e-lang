using E_Lang.Application.Common.Interfaces;
using E_Lang.Builder.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders
{
    public class BaseBuilder : IBaseBuilder
    {
        private readonly IAppDbContext _context;

        public BaseBuilder(IAppDbContext context)
        {
            _context = context;
        }

        public UserBuilder AddUser(out User user)
        {
            user = Entities.GetUser();
            return new UserBuilder(user, this, _context);
        }

        public CollectionBuilder<BaseBuilder> AddCollection(out Collection collection, Guid userId)
        {
            collection = Entities.GetCollection(userId);
            return new CollectionBuilder<BaseBuilder>(collection, this, _context);
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
