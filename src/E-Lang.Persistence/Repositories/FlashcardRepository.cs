using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class FlashcardRepository : Repository<Flashcard, FlashcardDto>, IFlashcardRepository
{
    public FlashcardRepository(IAppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<FlashcardDto?> GetByIdAsDtoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _entity
            .Where(fc => fc.Id == id)
            .ProjectToType<FlashcardDto>()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<FlashcardDto>> GetAllAsDtoAsync(CancellationToken cancellationToken = default)
    {
        return await _entity
            .ProjectToType<FlashcardDto>()
            .ToListAsync(cancellationToken);
    }
}