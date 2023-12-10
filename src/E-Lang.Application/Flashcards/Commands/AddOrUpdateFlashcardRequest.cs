using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Errors;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace E_Lang.Application.Flashcards.Commands;

public record AddOrUpdateFlashcardRequest : IRequest<FlashcardDto>
{
    public AddOrUpdateFlashcardDto Flashcard { get; set; }
}

public class AddOrUpdateFlashcardRequestHandler : IRequestHandler<AddOrUpdateFlashcardRequest, FlashcardDto>
{
    private readonly IUserService _userService;
    private readonly IFlashcardRepository _flashcardRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public AddOrUpdateFlashcardRequestHandler(IUserService userService, IFlashcardRepository flashcardRepository,
        IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _userService = userService;
        _flashcardRepository = flashcardRepository;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }
    
    public async Task<FlashcardDto> Handle(AddOrUpdateFlashcardRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();
        
        var flashcard = _mapper.Map<Flashcard>(request.Flashcard);
        
        FillInMissingProperties(flashcard, user.Id);
        
        _flashcardRepository.Update(flashcard);

        await _flashcardRepository.SaveAsync(cancellationToken);

        return await _flashcardRepository.GetByIdAsDtoAsync(flashcard.Id)
            ?? throw new NotFoundValidationException(nameof(Flashcard), nameof(Flashcard.Id), flashcard.Id.ToString());
    }
    
    private void FillInMissingProperties(Flashcard flashcard, Guid userId)
    {
        flashcard.LastStatusChangedOn = _dateTimeProvider.UtcNow;
        flashcard.OwnerId = userId;
    }
}