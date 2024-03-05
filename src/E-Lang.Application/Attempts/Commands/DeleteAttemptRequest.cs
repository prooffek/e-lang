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
    private readonly IAttemptStageRepository _attemptStageRepository;

    public DeleteAttemptRequestHandler(IAttemptRepository attemptRepository, IUserService userService, IAttemptStageRepository attemptStageRepository)
    {
        _attemptRepository = attemptRepository;
        _userService = userService;
        _attemptStageRepository = attemptStageRepository;
    }
    
    public async Task<Unit> Handle(DeleteAttemptRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new UserNotFoundException();

        if (await _attemptRepository.AnyAsync(a => a.Id == request.AttemptId && a.Collection.OwnerId != user.Id))
            throw new UnauthorizedException(user.Id, ActionTypes.Delete);

        var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken)
            ?? throw new NotFoundValidationException(nameof(Attempt), nameof(Attempt.Id), request.AttemptId.ToString());

        var attemptStages = attempt.AttemptStages?.Select(x => new AttemptStage {Id = x.Id}) 
                            ?? new List<AttemptStage>();

        _attemptRepository.Delete(new Attempt { Id = request.AttemptId });
        _attemptStageRepository.DeleteRange(attemptStages);

        await _attemptRepository.SaveAsync(cancellationToken);
        
        return Unit.Value;
    }
}