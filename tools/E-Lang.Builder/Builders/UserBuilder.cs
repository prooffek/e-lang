using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Builder.Builders;

public class UserBuilder : EntityBuilderBase<User, BaseBuilder>
{
    public UserBuilder(User entity, BaseBuilder parentBuilder, IAppDbContext context) 
        : base(entity, parentBuilder, context)
    {
    }

    public UserBuilder SetUsername(string userName)
    {
        _entity.UserName = userName;
        return this;
    }

    public UserBuilder SetEmail(string email)
    {
        _entity.Email = email;
        return this;
    }

    public CollectionBuilder<UserBuilder> AddCollection(out Collection collection)
    {
        collection = Entities.GetCollection(_entity.Id);
        return new CollectionBuilder<UserBuilder>(collection, this, _context);
    }
}