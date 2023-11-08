using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class CollectionRepository : Repository<Collection, CollectionCardDto>, ICollectionRepository
{
    public CollectionRepository(IAppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<CollectionCardDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _queryWithIncludes
            .Where(c => c.Id == id)
            .ProjectToType<CollectionCardDto>()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<CollectionCardDto>> GetAllAsDtoAsync(CancellationToken cancellationToken= default)
    {
        return await _queryWithIncludes
            .ProjectToType<CollectionCardDto>()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CollectionCardDto>> GetCollectionCardsAsync(Guid userId, Guid? parentCollectionId, CancellationToken cancellationToken)
    {
        return await _queryWithIncludes
            .Where(c => c.OwnerId == userId && c.ParentId == parentCollectionId)
            .OrderBy(c => c.Name)
            .ProjectToType<CollectionCardDto>()
            .ToListAsync(cancellationToken);
    }

    public async Task<CollectionDto?> GetCollectionAsDtoAsync(Guid collectionId, CancellationToken cancellationToken)
    {
        return await _queryWithIncludes
            .ProjectToType<CollectionDto>()
            .FirstOrDefaultAsync(c => c.Id == collectionId, cancellationToken);
    }

    public async Task<bool> IsUserCollectionOwner(Guid userId, Guid collectionId, CancellationToken cancellationToken)
    {
        return await _entity.AnyAsync(c => c.OwnerId == userId && c.Id == collectionId, cancellationToken);
    }

    protected override IQueryable<Collection> GetQueryWithIncludes()
    {
        return _entity
            .Include(c => c.Flashcards)
            .Include(c => c.Parent)
            .Include(c => c.Subcollections);
    }
}