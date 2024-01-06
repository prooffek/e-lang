using E_Lang.Application.Common.DTOs;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories;

public interface IAttemptRepository : IRepository<Attempt>, IRepositoryWithDto<Attempt, AttemptDto>
{
}