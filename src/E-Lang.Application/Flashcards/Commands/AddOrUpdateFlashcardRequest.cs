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
    private readonly IMeaningRepository _meaningRepository;
    private readonly IFlashcardBaseRepository _flashcardBaseRepository;

    public AddOrUpdateFlashcardRequestHandler(IUserService userService, IFlashcardRepository flashcardRepository,
        IDateTimeProvider dateTimeProvider, IMapper mapper, IMeaningRepository meaningRepository, IFlashcardBaseRepository flashcardBaseRepository)
    {
        _userService = userService;
        _flashcardRepository = flashcardRepository;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
        _meaningRepository = meaningRepository;
        _flashcardBaseRepository = flashcardBaseRepository;
    }
    
    public async Task<FlashcardDto> Handle(AddOrUpdateFlashcardRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUser(cancellationToken)
            ?? throw new UserNotFoundException();
        
        var flashcard = _mapper.Map<Flashcard>(request.Flashcard);

        Guid? prevFlashcardBaseId = await GetPrevFlashcardBaseId(request.Flashcard.FlashcardId, cancellationToken);

        FillInMissingProperties(flashcard, user.Id);

        _flashcardRepository.Update(flashcard);

        await RemoveUnusedMeanings(flashcard.FlashcardBaseId, request.Flashcard.Meanings, cancellationToken);

        await _flashcardRepository.SaveAsync(cancellationToken);

        RemoveUnusedFlashcardBase(flashcard.FlashcardBaseId, prevFlashcardBaseId, cancellationToken);

        return await _flashcardRepository.GetByIdAsDtoAsync(flashcard.Id)
            ?? throw new NotFoundValidationException(nameof(Flashcard), nameof(Flashcard.Id), flashcard.Id.ToString());
    }
    
    private void FillInMissingProperties(Flashcard flashcard, Guid userId)
    {
        flashcard.LastStatusChangedOn = _dateTimeProvider.UtcNow;
        flashcard.OwnerId = userId;
    }

    private async Task<Guid?> GetPrevFlashcardBaseId(Guid? flashcardId, CancellationToken cancellationToken)
    {
        if (flashcardId.HasValue)
        {
            return await _flashcardRepository.GetFlashcardBaseIdAsync(flashcardId.Value, cancellationToken);
        }

        return null;
    }

    private async Task RemoveUnusedMeanings(Guid flashcardBaseId, IEnumerable<AddOrUpdateMeaningDto> meanings, CancellationToken cancellationToken)
    {
        if (flashcardBaseId != Guid.Empty)
        {
            var meaningsToDelete = (await _meaningRepository.GetByFlashcardBaseIdAsync(flashcardBaseId, cancellationToken))
                .Where(x => !meanings.Select(m => m.Id).Contains(x.Id));

            if (meaningsToDelete.Any())
            {
                _meaningRepository.DeleteRange(meaningsToDelete);
            }
        }
    }

    private async void RemoveUnusedFlashcardBase(Guid flashcardBaseId, Guid? prevFlashcardBaseId, CancellationToken cancellationToken)
    {
        if (prevFlashcardBaseId.HasValue 
            && flashcardBaseId != prevFlashcardBaseId.Value 
            && !await _flashcardRepository.AnyAsync(f => f.FlashcardBaseId == prevFlashcardBaseId, cancellationToken))
        {
            var flashcardBase = await _flashcardBaseRepository.GetByIdAsync(prevFlashcardBaseId.Value, cancellationToken)
            ?? throw new ArgumentNullException(nameof(Flashcard.FlashcardBaseId));

            _flashcardBaseRepository.Delete(flashcardBase);
            await _flashcardBaseRepository.SaveAsync();
        }
    }
}