using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Models;

namespace E_Lang.Application.Services
{
    public class FlashcardStateService : IFlashcardStateService
    {
        private readonly IFlashcardStateRepository _flashcardStateRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FlashcardStateService(IFlashcardStateRepository flashcardStateRepository, IDateTimeProvider dateTimeProvider)
        {
            _flashcardStateRepository = flashcardStateRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task UpdateFlashcardState(Guid flashcardStateId, bool isAnswerCorrect, Attempt attempt, CancellationToken cancellationToken)
        {
            var flashcardState =
                await GetUpdatedFlashcardState(flashcardStateId, isAnswerCorrect, attempt, cancellationToken);

            _flashcardStateRepository.Update(flashcardState);
            await _flashcardStateRepository.SaveAsync(cancellationToken);

            if (flashcardState is CompletedFlashcardState cfs)
            {
                var oldFlashcardState = attempt.CurrentStage?.Flashcards?.First(x => x.Id == flashcardStateId)
                                        ?? throw new ArgumentOutOfRangeException($"Flashcard state with id '{flashcardStateId} not found in the current stage.");

                attempt.CurrentStage.Flashcards.Remove(oldFlashcardState);
                attempt.CurrentStage.Flashcards.Add(cfs);
            }
        }

        private async Task<FlashcardState> GetUpdatedFlashcardState(Guid flashcardStateId, bool isAnswerCorrect, Attempt attempt, CancellationToken cancellationToken)
        {
            var flashcardState =
                await _flashcardStateRepository.GetByIdAsync(flashcardStateId, cancellationToken)
                ?? throw new NotFoundValidationException(nameof(FlashcardState),
                    nameof(FlashcardState.Id), flashcardStateId.ToString());

            var data = new NextStateData()
            {
                UtcNow = _dateTimeProvider.UtcNow,
                IsAnswerCorrect = isAnswerCorrect,
                Attempt = attempt
            };

            return flashcardState.GetNextState(data);
        }
    }
}
