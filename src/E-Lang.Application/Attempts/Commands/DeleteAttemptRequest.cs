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

    public DeleteAttemptRequestHandler(IAttemptRepository attemptRepository, IUserService userService)
    {
        _attemptRepository = attemptRepository;
        _userService = userService;
    }
    
    public async Task<Unit> Handle(DeleteAttemptRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
                   ?? throw new UserNotFoundException();

        if (await _attemptRepository.AnyAsync(a => a.Id == request.AttemptId && a.Collection.OwnerId != user.Id))
            throw new UnauthorizedException(user.Id, ActionTypes.Delete);

        _attemptRepository.Delete(new Attempt { Id = request.AttemptId });
        await _attemptRepository.SaveAsync(cancellationToken);
        
        return Unit.Value;
    }
}