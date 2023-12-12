using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.Interfaces.Repositories
{
    public interface IMeaningRepository : IRepository<Meaning>
    {
        Task<IEnumerable<Meaning>> GetByFlashcardBaseIdAsync(Guid flashcardBaseId, CancellationToken cancellationToken);
    }
}
