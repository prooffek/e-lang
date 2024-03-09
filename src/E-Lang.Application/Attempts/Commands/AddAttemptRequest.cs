using System.Collections;
using System.Runtime.InteropServices;
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
        private readonly IQuizTypeRepository _quizTypeRepository;
        private readonly IAttemptStageService _attemptStageService;

        public AddAttemptRequestHandler(IAttemptRepository attemptRepository, IUserService userService,
            IMapper mapper, IFlashcardRepository flashcardRepository, IQuizTypeRepository quizTypeRepository,
            IAttemptStageService attemptStageService)
        {
            _attemptRepository = attemptRepository;
            _userService = userService;
            _mapper = mapper;
            _flashcardRepository = flashcardRepository;
            _quizTypeRepository = quizTypeRepository;
            _attemptStageService = attemptStageService;
        }

        public async Task<AttemptDto> Handle(AddAttemptRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUser(cancellationToken)
                ?? throw new UserNotFoundException();

            var flashcards = await _flashcardRepository.GetFlashcardsByCollectionId(request.Attempt.CollectionId, cancellationToken);

            var attempt = _mapper.Map<Attempt>(request.Attempt)
                          ?? throw new Exception($"Cannot convert from {nameof(AddAttemptDto)} to {nameof(Attempt)}");
            
            await SetQuizTypes(attempt, request.Attempt.QuizTypes);

            var attemptStage = _attemptStageService.GetAttemptStage(attempt.Id, flashcards, request.Attempt.Order, request.Attempt.MaxFlashcardsPerStage);

            await UpdateAttempt(attempt, attemptStage, cancellationToken);

            return await _attemptRepository.GetByIdAsDtoAsync(attempt.Id, cancellationToken)
                ?? throw new NullOrEmptyValidationException(nameof(Attempt), nameof(Attempt.Id), ActionTypes.Get);
        }

        private async Task UpdateAttempt(Attempt attempt, AttemptStage attemptStage, CancellationToken cancellationToken)
        {
            attempt.AttemptStages ??= new List<AttemptStage>();
            attempt.AttemptStages.Add(attemptStage);
            _attemptRepository.Add(attempt);

            await _attemptRepository.SaveAsync(cancellationToken);
        }

        private async Task SetQuizTypes(Attempt attempt, IEnumerable<QuizTypeDto>? selectedQuizTypes)
        {
            var quizTypes = selectedQuizTypes is not null && selectedQuizTypes.Any() 
                ? _mapper.Map<IList<QuizType>>(selectedQuizTypes)
                : (await _quizTypeRepository.GetAllAsync()).ToList()
                    ?? throw new NullReferenceException("No quiz type found.");

            if (quizTypes.All(x => !x.IsFirst))
            {
                var firstQuiz = await _quizTypeRepository.GetQuizTypeWithFlashIsFirst();

                if (!quizTypes.Select(x => x.Id).ToHashSet().Contains(firstQuiz.Id))
                {
                    quizTypes.Add(firstQuiz);
                }
            }

            attempt.AttemptQuizTypes = quizTypes.Select(x => new AttemptQuizType {AttemptId = attempt.Id, QuizTypeId = x.Id}).ToList();
        }
    }
}
