using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.Services
{
    public class AttemptStageService : IAttemptStageService
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IAttemptStageRepository _attemptStageRepository;
        private readonly IFlashcardStateRepository _flashcardStateRepository;
        private readonly IAttemptRepository _attemptRepository;

        public AttemptStageService(IFlashcardRepository flashcardRepository, IAttemptStageRepository attemptStageRepository,
            IFlashcardStateRepository flashcardStateRepository, IAttemptRepository attemptRepository)
        {
            _flashcardRepository = flashcardRepository;
            _attemptStageRepository = attemptStageRepository;
            _flashcardStateRepository = flashcardStateRepository;
            _attemptRepository = attemptRepository;
        }

        public AttemptStage GetAttemptStage(Guid attemptId, IEnumerable<Flashcard> flashcards, FlashcardOrder order, int maxFlashcards)
        {
            IEnumerable<FlashcardState> flashcardStates = OrderFlashcards(flashcards, order)
                .Take(maxFlashcards)
                .Select(f => new InitFlashcardState(f));

            return new AttemptStage
            {
                AttemptId = attemptId,
                Flashcards = flashcardStates.ToList(),
            };
        }

        public async Task<AttemptStage?> GetNextAttemptStage(Attempt attempt, CancellationToken cancellationToken)
        {
            var completedFlashcardIds = attempt.CompletedFlashcards?.Select(x => x.Id).ToHashSet() 
                ?? new HashSet<Guid>();

            var flashcards = await _flashcardRepository.GetFlashcardsByCollectionId(attempt.CollectionId, cancellationToken);
            var availableFlashcards = flashcards.Where(x => !completedFlashcardIds.Contains(x.Id));

            return availableFlashcards.Any() 
                ? GetAttemptStage(attempt.Id, availableFlashcards, attempt.Order, attempt.MaxFlashcardsPerStage)
                : null;
        }

        public async Task CompleteAttemptStage(Attempt attempt, CancellationToken cancellationToken)
        {
            var currentStage = attempt.CurrentStage;
            currentStage.Stage = AttemptStageType.Complete;
            _attemptStageRepository.Update(currentStage);

            var nextStage = await GetNextAttemptStage(attempt, cancellationToken);

            if (nextStage != null)
            {
                _attemptStageRepository.Add(nextStage);
                await _flashcardStateRepository.SaveAsync(cancellationToken);
            }

            attempt.AttemptStages?.Add(nextStage);
            await _attemptRepository.SaveAsync(cancellationToken);
        }

        private IEnumerable<Flashcard> OrderFlashcards(IEnumerable<Flashcard> flashcards, FlashcardOrder order)
        {
            switch (order)
            {
                case FlashcardOrder.Random:
                    return flashcards.OrderBy(r => Guid.NewGuid());
                case FlashcardOrder.CreationDateDesc:
                    return flashcards.OrderByDescending(f => f.CreatedOn);
                case FlashcardOrder.CreationDateAsc:
                    return flashcards.OrderBy(f => f.CreatedOn);
                case FlashcardOrder.AlphabeticalAsc:
                    return flashcards.OrderBy(f => f.FlashcardBase.WordOrPhrase);
                default:
                    return flashcards.OrderByDescending(f => f.FlashcardBase.WordOrPhrase);
            }
        }
    }
}
