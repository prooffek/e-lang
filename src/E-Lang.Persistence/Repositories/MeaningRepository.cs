using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories
{
    public class MeaningRepository : Repository<Meaning>, IMeaningRepository
    {
        public MeaningRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
        {
        }

        public async Task<IEnumerable<Meaning>> GetByFlashcardBaseIdAsync(Guid flashcardBaseId, CancellationToken cancellationToken)
        {
            return await _entity
                .Where(m => m.FlashcardBaseId == flashcardBaseId)
                .ToListAsync(cancellationToken);
        }
    }
}
