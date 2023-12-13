using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using MediatR;

namespace E_Lang.Application.Flashcards.Commands
{
    public record RemoveFlashcardsRequest : IRequest
    {
        public IEnumerable<Guid>? FlashcardIds { get; set; }
    }

    public class RemoveFlashcardsRequestHanlder : IRequestHandler<RemoveFlashcardsRequest>
    {
        private readonly IUserService _userService;
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IFlashcardService _flashcardService;

        public RemoveFlashcardsRequestHanlder(IUserService userService, IFlashcardRepository flashcardRepository, IFlashcardService flashcardService)
        {
            _userService = userService;
            _flashcardRepository = flashcardRepository;
            _flashcardService = flashcardService;
        }

        public async Task<Unit> Handle(RemoveFlashcardsRequest request, CancellationToken cancellationToken)
        {
            if (request.FlashcardIds is null || !request.FlashcardIds.Any())
                throw new NullOrEmptyValidationException(nameof(Flashcard), nameof(RemoveFlashcardsRequest.FlashcardIds), ActionTypes.Delete);

            var user = await _userService.GetCurrentUser(cancellationToken)
                ?? throw new UserNotFoundException();

            if (await _flashcardRepository.AnyAsync(f => request.FlashcardIds.ToHashSet().Contains(f.Id) && f.OwnerId != user.Id))
                throw new UnauthorizedException(user.Id, ActionTypes.Delete);

            var flashcards = await _flashcardRepository.GetFlashcardsByIdAsync(request.FlashcardIds.ToHashSet(), cancellationToken);

            var flashcardBaseIds = flashcards.Select(f => f.FlashcardBaseId).Distinct();

            _flashcardRepository.DeleteRange(flashcards);
            await _flashcardRepository.SaveAsync(cancellationToken);

            await _flashcardService.RemoveUnusedFlashcardBases(flashcardBaseIds, cancellationToken);

            await _flashcardRepository.SaveAsync(cancellationToken);

            ThrowIfNotFound(flashcards.Select(f => f.Id), request.FlashcardIds);

            return Unit.Value;
        }

        private void ThrowIfNotFound(IEnumerable<Guid> flashcardIds, IEnumerable<Guid> idsToRemove)
        {
            var notFoundFlashcardIds = idsToRemove
                .Where(id => !flashcardIds
                    .ToHashSet()
                    .Contains(id));

            if (notFoundFlashcardIds.Any())
            {
                throw new NotFoundValidationException(nameof(Flashcard), nameof(Flashcard.Id), string.Join(',', notFoundFlashcardIds));
            }
        }
    }
}
