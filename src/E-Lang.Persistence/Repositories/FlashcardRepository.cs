using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class FlashcardRepository : RepositoryWithDto<Flashcard, FlashcardDto>, IFlashcardRepository
{
    public FlashcardRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
    {
    }

    public override async Task<FlashcardDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _entity
            .Include(f => f.FlashcardBase)
                .ThenInclude(f => f.Meanings)
            .Include(f => f.Collection)
            .Where(fc => fc.Id == id)
            .ProjectToType<FlashcardDto>()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<FlashcardDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default)
    {
        return await _entity
            .Include(f => f.FlashcardBase)
                .ThenInclude(f => f.Meanings)
            .Include(f => f.Collection)
            .ProjectToType<FlashcardDto>()
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid?> GetFlashcardBaseIdAsync(Guid flashcardId, CancellationToken cancellationToken)
    {
        return (await _entity
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == flashcardId, cancellationToken))
            ?.FlashcardBaseId;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByIdAsync(HashSet<Guid> ids, CancellationToken cancellationToken)
    {
        return await _entity
            .Where(f => ids.Contains(f.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByCollectionId(Guid collectionId, CancellationToken cancellationToken)
    {
        return await _queryWithIncludes
            .Where(f => f.CollectionId == collectionId)
            .ToListAsync(cancellationToken);
    }

    protected override IQueryable<Flashcard> GetQueryWithIncludes()
    {
        return _entity
            .Include(f => f.FlashcardBase)
                .ThenInclude(fb => fb.Meanings);
    }
}