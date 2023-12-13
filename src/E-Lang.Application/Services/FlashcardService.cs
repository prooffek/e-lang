using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Services
{
    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IMeaningRepository _meaningRepository;
        private readonly IFlashcardBaseRepository _flashcardBaseRepository;

        public FlashcardService(IFlashcardRepository flashcardRepository, IMeaningRepository meaningRepository,
            IFlashcardBaseRepository flashcardBaseRepository)
        {
            _flashcardRepository = flashcardRepository;
            _meaningRepository = meaningRepository;
            _flashcardBaseRepository = flashcardBaseRepository;
        }

        public async Task RemoveUnusedFlashcardBase(Guid currentflashcardBaseId, Guid? prevFlashcardBaseId, CancellationToken cancellationToken)
        {
            if (prevFlashcardBaseId.HasValue && currentflashcardBaseId != prevFlashcardBaseId.Value
                && !await _flashcardRepository.AnyAsync(f => f.FlashcardBaseId == prevFlashcardBaseId, cancellationToken))
            {
                _flashcardBaseRepository.Delete(new FlashcardBase { Id = prevFlashcardBaseId.Value});
            }
        }

        public async Task RemoveUnusedFlashcardBase(Guid flashcardId, Guid flashcardBaseId, CancellationToken cancellationToken)
        {
            if (!await _flashcardRepository.AnyAsync(f => f.FlashcardBaseId == flashcardBaseId && f.Id != flashcardId, cancellationToken))
            {
                _flashcardBaseRepository.Delete(new FlashcardBase { Id = flashcardBaseId });
            }
        }

        public async Task RemoveUnusedFlashcardBases(IEnumerable<Guid> flashcardBaseIds, CancellationToken cancellationToken)
        {
            foreach (var id in flashcardBaseIds)
            {
                if (!await _flashcardRepository.AnyAsync(f => f.FlashcardBaseId == id, cancellationToken))
                {
                    _flashcardBaseRepository.Delete(new FlashcardBase { Id = id });
                }
            }
        }

        public async Task RemoveUnusedMeanings(Guid flashcardBaseId, IEnumerable<AddOrUpdateMeaningDto> meanings, CancellationToken cancellationToken)
        {
            if (flashcardBaseId != Guid.Empty)
            {
                var meaningsToDelete = (await _meaningRepository.GetByFlashcardBaseIdAsync(flashcardBaseId, cancellationToken))
                    .Where(x => !meanings.Select(m => m.Id).Contains(x.Id));

                if (meaningsToDelete.Any())
                {
                    _meaningRepository.DeleteRange(meaningsToDelete);
                }
            }
        }
    }
}
