using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Lang.Persistence.Repositories
{
    public class QuizTypeRepository : Repository<QuizType>, IQuizTypeRepository
    {
        public QuizTypeRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) 
            : base(dbContext, dateTimeProvider)
        {
        }

        public async Task<QuizType> GetQuizTypeWithFlashIsFirst()
        {
            return await _entity.SingleOrDefaultAsync(x => x.IsFirst)
                ?? throw new NotFoundValidationException(nameof(QuizType), nameof(QuizType.IsFirst), "true");
        }
    }
}
