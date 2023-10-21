using Microsoft.EntityFrameworkCore;

namespace E_Lang.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<T> GetDbSet<T>() where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}