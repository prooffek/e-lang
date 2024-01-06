using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Persistence.Repositories;

public class FlashcardStateRepository : Repository<FlashcardState>, IFlashcardStateRepository
{
    public FlashcardStateRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) 
        : base(dbContext, dateTimeProvider)
    {
    }
}