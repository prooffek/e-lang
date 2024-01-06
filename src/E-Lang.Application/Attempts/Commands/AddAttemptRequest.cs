using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace E_Lang.Application.Attempts.Commands
{
    public record AddAttemptRequest : IRequest<AttemptDto>
    {
        public AddAttemptDto Attempt { get; set; }
    }

    public class AddAttemptRequestHandler : IRequestHandler<AddAttemptRequest, AttemptDto>
    {
        private readonly IAttemptRepository _attemptRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IAttemptStageService _attemptStageService;

        public AddAttemptRequestHandler(IAttemptRepository attemptRepository, IUserService userService,
            IMapper mapper, IFlashcardRepository flashcardRepository,
            IAttemptStageService attemptStageService)
        {
            _attemptRepository = attemptRepository;
            _userService = userService;
            _mapper = mapper;
            _flashcardRepository = flashcardRepository;
            _attemptStageService = attemptStageService;
        }

        public async Task<AttemptDto> Handle(AddAttemptRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUser(cancellationToken)
                ?? throw new UserNotFoundException();

            var flashcards = await _flashcardRepository.GetFlashcardsByCollectionId(request.Attempt.CollectionId, cancellationToken);

            var attemptStage = _attemptStageService.GetInitAttemptStage(flashcards, request.Attempt.Order, request.Attempt.MaxFlashcardsPerStage);

            var attempt = _mapper.Map<Attempt>(request.Attempt)
                ?? throw new Exception($"Cannot convert from {nameof(AddAttemptDto)} to {nameof(Attempt)}");

            attempt.CurrentStage = attemptStage;

            _attemptRepository.Add(attempt);
            await _attemptRepository.SaveAsync();

            return await _attemptRepository.GetByIdAsDtoAsync(attempt.Id, cancellationToken)
                ?? throw new NullOrEmptyValidationException(nameof(Attempt), nameof(Attempt.Id), ActionTypes.Get);
        }
    }
}
