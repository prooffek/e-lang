using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.Services
{
    public class AttemptStageService : IAttemptStageService
    {
        public AttemptStage GetInitAttemptStage(IEnumerable<Flashcard> flashcards, FlashcardOrder order, int maxFlashcards)
        {
            IEnumerable<FlashcardState> flashcardStates = OrderFlashcards(flashcards, order)
                .Take(maxFlashcards)
                .Select(f => new InitFlashcardState(f));

            return new AttemptStage
            {
                Flashcards = flashcardStates.ToList(),
            };
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
