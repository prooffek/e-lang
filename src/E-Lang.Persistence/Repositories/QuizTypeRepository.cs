using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Persistence.Repositories
{
    public class QuizTypeRepository : Repository<QuizType>, IQuizTypeRepository
    {
        public QuizTypeRepository(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider) 
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
