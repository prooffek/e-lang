using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using MediatR;

namespace E_Lang.Application.Exercises.Requests;

public record GetNextExerciseRequest : IRequest<NextExerciseDto>
{
    public Guid AttemptId { get; set; }
    public Guid? FlashcardStateId { get; set; }
    public bool? IsAnswerCorrect { get; set; }
}

public class GetNextExerciseRequestHandler : IRequestHandler<GetNextExerciseRequest, NextExerciseDto>
{
    private readonly IUserService _userService;
    private readonly IAttemptRepository _attemptRepository;
    private readonly IExerciseService _exerciseService;
    private readonly IAttemptStageService _attemptStageService;
    private readonly IFlashcardStateService _flashcardStateService;

    public GetNextExerciseRequestHandler(IUserService userService, IAttemptRepository attemptRepository, IExerciseService exerciseService,
        IAttemptStageService attemptStageService, IFlashcardStateService flashcardStateService)
    {
        _userService = userService;
        _attemptRepository = attemptRepository;
        _exerciseService = exerciseService;
        _attemptStageService = attemptStageService;
        _flashcardStateService = flashcardStateService;
    }
    
    public async Task<NextExerciseDto> Handle(GetNextExerciseRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();

        var attempt = await GetAttempt(request.AttemptId, user.Id, cancellationToken);

        if (IsFlashcardStateModified(request))
        {
            await _flashcardStateService.UpdateFlashcardState(request.FlashcardStateId!.Value, request.IsAnswerCorrect!.Value, attempt, cancellationToken);
        }

        var nextExercise = await _exerciseService.GetNextExercise(attempt, request.FlashcardStateId, cancellationToken);

        if (nextExercise is null)
        {
            await _attemptStageService.CompleteAttemptStage(attempt, cancellationToken);
        }

        return nextExercise is null 
        ? new NextExerciseDto { IsStageComplete = true }
        : new NextExerciseDto { ExerciseDto = nextExercise };
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
}