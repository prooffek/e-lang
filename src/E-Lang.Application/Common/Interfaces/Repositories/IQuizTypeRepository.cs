using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories
{
    public interface IQuizTypeRepository : IRepository<QuizType>
    {
        Task<QuizType> GetQuizTypeWithFlashIsFirst();
    }
}
