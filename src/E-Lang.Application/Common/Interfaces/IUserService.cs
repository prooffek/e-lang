using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces;

public interface IUserService
{
    Task<User?> GetCurrentUser(CancellationToken cancellationToken);
}