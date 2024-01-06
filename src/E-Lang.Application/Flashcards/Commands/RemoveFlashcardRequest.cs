using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using MediatR;

namespace E_Lang.Application.Flashcards.Commands
{
    public record RemoveFlashcardRequest : IRequest
    {
        public Guid? FlashcardId { get; set; }
    }

    public class RemoveFlashcardRequestHandler : IRequestHandler<RemoveFlashcardRequest>
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IUserService _userService;
        private readonly IFlashcardService _flashcardService;

        public RemoveFlashcardRequestHandler(IFlashcardRepository flashcardRepository, IUserService userService, 
            IFlashcardService flashcardService)
        {
            _flashcardRepository = flashcardRepository;
            _userService = userService;
            _flashcardService = flashcardService;
        }

        public async Task<Unit> Handle(RemoveFlashcardRequest request, CancellationToken cancellationToken)
        {
            if (!request.FlashcardId.HasValue || request.FlashcardId.Value == Guid.Empty)
                throw new NullOrEmptyValidationException(nameof(Flashcard), nameof(RemoveFlashcardRequest.FlashcardId), ActionTypes.Delete);

            var user = await _userService.GetCurrentUser(cancellationToken);

            if (user == null)
                throw new UserNotFoundException();

            var flashcard = await _flashcardRepository.GetByIdAsync(request.FlashcardId.Value, cancellationToken)
                ?? throw new NotFoundValidationException(nameof(Flashcard), nameof(Flashcard.Id), request.FlashcardId.ToString());

            if (user.Id != flashcard.OwnerId)
                throw new UnauthorizedException(user.Id, ActionTypes.Delete);

            _flashcardRepository.Delete(flashcard);
            await _flashcardService.RemoveUnusedFlashcardBase(flashcard.Id, flashcard.FlashcardBase, cancellationToken);
            await _flashcardRepository.SaveAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
