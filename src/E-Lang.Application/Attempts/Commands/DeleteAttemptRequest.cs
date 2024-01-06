using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Enums;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using MediatR;

namespace E_Lang.Application.Attempts.Commands;

public record DeleteAttemptRequest : IRequest
{
    public Guid AttemptId { get; set; }
};

public class DeleteAttemptRequestHandler : IRequestHandler<DeleteAttemptRequest>
{
    private readonly IAttemptRepository _attemptRepository;
    private readonly IUserService _userService;
    private readonly ICollectionRepository _collectionRepository;
    private readonly IAttemptStageRepository _attemptStageRepository;
    private readonly IFlashcardStateRepository _flashcardStateRepository;

    public DeleteAttemptRequestHandler(IAttemptRepository attemptRepository, IUserService userService, 
        ICollectionRepository collectionRepository, IAttemptStageRepository attemptStageRepository,
        IFlashcardStateRepository flashcardStateRepository)
    {
        _attemptRepository = attemptRepository;
        _userService = userService;
        _collectionRepository = collectionRepository;
        _attemptStageRepository = attemptStageRepository;
        _flashcardStateRepository = flashcardStateRepository;
    }
    
    public async Task<Unit> Handle(DeleteAttemptRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new UserNotFoundException();

        var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId, default)
            ?? throw new NotFoundValidationException(nameof(Attempt), nameof(Attempt.Id), request.AttemptId.ToString());

        if (attempt.Collection?.OwnerId != user.Id)
        {
            throw new UnauthorizedException(user.Id, ActionTypes.Delete);
        }
        
        _attemptRepository.Delete(attempt);
        _attemptStageRepository.Delete(attempt.CurrentStage);
        _flashcardStateRepository.DeleteRange(attempt.CurrentStage.Flashcards);
        await _attemptRepository.SaveAsync(cancellationToken);
        
        return Unit.Value;
    }
}