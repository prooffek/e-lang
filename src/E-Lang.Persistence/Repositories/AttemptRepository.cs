using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class AttemptRepository : RepositoryWithDto<Attempt, AttemptDto>, IAttemptRepository
{
    public AttemptRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
    {
    }

    public override async Task<IEnumerable<AttemptDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default)
    {
        return await _queryWithIncludes
            .ProjectToType<AttemptDto>()
            .ToListAsync(cancellationToken);
    }

    public override async Task<AttemptDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _queryWithIncludes
            .ProjectToType<AttemptDto>()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    protected override IQueryable<Attempt> GetQueryWithIncludes()
    {
        return _entity
            .Include(a => a.Properties)
            .Include(a => a.QuizTypes)
            .Include(a => a.CompletedFlashcards)
            .Include(a => a.Collection)
            .Include(a => a.CurrentStage)
                .ThenInclude(s => s.Flashcards);
    }
}