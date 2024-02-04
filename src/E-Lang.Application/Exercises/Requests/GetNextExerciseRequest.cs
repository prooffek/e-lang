using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Application.Models;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using E_Lang.Domain.Models;
using MediatR;

namespace E_Lang.Application.Exercises.Requests;

public record GetNextExerciseRequest : IRequest<ExerciseDto>
{
    public Guid AttemptId { get; set; }
    public Guid? FlashcardStateId { get; set; }
    public bool? IsAnswerCorrect { get; set; }
}

public class GetNextExerciseRequestHandler : IRequestHandler<GetNextExerciseRequest, ExerciseDto>
{
    private readonly IUserService _userService;
    private readonly IFlashcardStateRepository _flashcardStateRepository;
    private readonly IAttemptRepository _attemptRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IExerciseService _exerciseService;
    private readonly IQuizTypeRepository _quizTypeRepository;

    public GetNextExerciseRequestHandler(IUserService userService, IFlashcardStateRepository flashcardStateRepository,
        IAttemptRepository attemptRepository, IDateTimeProvider dateTimeProvider, IExerciseService exerciseService,
        IQuizTypeRepository quizTypeRepository)
    {
        _userService = userService;
        _flashcardStateRepository = flashcardStateRepository;
        _attemptRepository = attemptRepository;
        _dateTimeProvider = dateTimeProvider;
        _exerciseService = exerciseService;
        _quizTypeRepository = quizTypeRepository;
    }
    
    public async Task<ExerciseDto> Handle(GetNextExerciseRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();

        var attempt = await GetAttempt(request.AttemptId, user.Id, cancellationToken);

        if (IsFlashcardStateModified(request))
        {
            await UpdateFlashcardState(request.FlashcardStateId.Value, request.IsAnswerCorrect.Value, attempt, cancellationToken);
        }

        return await GetNextExercise(attempt, request.FlashcardStateId, cancellationToken);
    }

    private async Task<Attempt> GetAttempt(Guid attemptId, Guid userId, CancellationToken cancellationToken)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId, cancellationToken)
            ?? throw new NotFoundValidationException(nameof(Attempt), nameof(Attempt.Id), attemptId.ToString());

        ValidateAttempt(attempt, userId, cancellationToken);

        return attempt;
    }

    private void ValidateAttempt(Attempt attempt, Guid userId, CancellationToken cancellationToken)
    {
        if (attempt.Collection?.OwnerId != userId)
            throw new UnauthorizedException(userId, ActionTypes.Get);

        if (attempt.CurrentStage.Flashcards.All(fs => fs is CompletedFlashcardState))
            throw new WrongStateException(FlashcardStatus.InProgress, FlashcardStatus.Learnt, nameof(FlashcardState));
    }

    private bool IsFlashcardStateModified(GetNextExerciseRequest request)
    {
        return request.FlashcardStateId.HasValue
            && request.FlashcardStateId != Guid.Empty 
            && request.IsAnswerCorrect.HasValue;
    }

    private async Task UpdateFlashcardState(Guid flashcardStateId, bool isAnswerCorrect, Attempt attempt, CancellationToken cancellationToken)
    {
        var flashcardState =
                await GetUpdatedFlashcardState(flashcardStateId, isAnswerCorrect, attempt, cancellationToken);

        _flashcardStateRepository.Update(flashcardState);
        await _flashcardStateRepository.SaveAsync(cancellationToken);
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

    private async Task<FlashcardState> GetRandomFlashcardState(IEnumerable<FlashcardState> flashcards, Guid? flashcardStateId, CancellationToken cancellationToken)
    {
        var flashcardStates = flashcards.Where(f => !(f is CompletedFlashcardState));

        FlashcardState? flashcardState;

        if (!flashcardStates.Any())
            throw new ArgumentOutOfRangeException();

        if (flashcardStates.Count() == 1)
        {
            flashcardState = flashcardStates.First();
        }
        else
        {
            flashcardState = flashcards
                .FirstOrDefault(x => x is InProgressFlashcardState f && f.ShowAgainOn.HasValue && f.ShowAgainOn <= _dateTimeProvider.UtcNow)

                ?? flashcards
                    .Where(f => !(f is CompletedFlashcardState) && (!flashcardStateId.HasValue || f.Id != flashcardStateId))
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefault();
        }

        if (flashcardState is null)
            throw new ArgumentOutOfRangeException();

        return await _flashcardStateRepository.GetByIdAsync(flashcardState.Id, cancellationToken)
            ?? throw new NullReferenceException($"Flashcard state with Id {flashcardState.Id} not found.");
    }

    private async Task<ExerciseDto> GetNextExercise(Attempt attempt, Guid? flashcardStateId, CancellationToken cancellationToken)
    {
        var nextFlashcard = await GetRandomFlashcardState(attempt.CurrentStage.Flashcards, flashcardStateId, cancellationToken);
        var nextQuiz = nextFlashcard.GetQuiz(attempt);

        ExerciseData exerciseData = new(attempt.Id, nextFlashcard.Id, attempt.CollectionId, nextFlashcard.Flashcard.FlashcardBase, nextQuiz);

        return await _exerciseService.GetExercise(exerciseData, cancellationToken);
    }
}