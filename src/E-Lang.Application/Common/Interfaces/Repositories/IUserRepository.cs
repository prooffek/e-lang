using E_Lang.Application.Common.DTOs;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>, IRepositoryWithDto<User, UserDto>
{
    Task<User?> GetUserByUserName(string userName, CancellationToken cancellationToken = default);
}