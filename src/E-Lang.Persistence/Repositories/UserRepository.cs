using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class UserRepository : Repository<User, UserDto>, IUserRepository
{
    public UserRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
    {
    }

    public override async Task<UserDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _queryWithIncludes
            .Where(u => u.Id == id)
            .ProjectToType<UserDto>()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<UserDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default)
    {
        return await _queryWithIncludes
            .ProjectToType<UserDto>()
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserByUserName(string userName, CancellationToken cancellationToken = default)
    {
        return await _entity
            .SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}