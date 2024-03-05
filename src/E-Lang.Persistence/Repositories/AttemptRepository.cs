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
        return (await _queryWithIncludes
            .ToListAsync(cancellationToken))
            .Adapt<IEnumerable<AttemptDto>>();
    }

    public override async Task<AttemptDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await _queryWithIncludes
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken))
            .Adapt<AttemptDto>();
    }

    protected override IQueryable<Attempt> GetQueryWithIncludes()
    {
        return _entity
            .Include(a => a.Properties)
            .Include(a => a.AttemptQuizTypes)
                .ThenInclude(qt => qt.QuizType)
            .Include(a => a.Collection)
                .ThenInclude(x => x.Flashcards)
            .Include(a => a.AttemptStages)
                .ThenInclude(s => s.Flashcards)
                    .ThenInclude(fs => fs.Flashcard)
                        .ThenInclude(f => f.FlashcardBase)
                            .ThenInclude(fb => fb.Meanings);
    }
}