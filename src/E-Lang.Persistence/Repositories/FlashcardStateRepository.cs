using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories;

public class FlashcardStateRepository : Repository<FlashcardState>, IFlashcardStateRepository
{
    public FlashcardStateRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) 
        : base(dbContext, dateTimeProvider)
    {
    }

    public override void Update(FlashcardState entity)
    {
        base.Update(entity);
        UpdateEntityType(entity);
    }

    protected override IQueryable<FlashcardState> GetQueryWithIncludes()
    {
        return _entity
            .Include(fs => fs.Flashcard)
                .ThenInclude(f => f.FlashcardBase)
                    .ThenInclude(fb => fb.Meanings)
            .Include(nameof(InProgressFlashcardState.CurrentQuizType))
            .Include(nameof(InProgressFlashcardState.CompletedQuizTypes))
            .Include(nameof(InProgressFlashcardState.SeenQuizTypes))
            .Include(nameof(InProgressFlashcardState.ExcludedQuizTypes))
            .AsQueryable();
    }

    private void UpdateEntityType<TEntity>(TEntity entity, string columnName = "Discriminator") where TEntity : EntityBase
    {
        var prop = _dbContext.Entry(entity).Property(columnName);
        prop.CurrentValue = entity.GetType().Name;
        prop.IsModified = true;
    }
}