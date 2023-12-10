using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using FluentValidation;

namespace E_Lang.Application.Validators;

public class AddOrUpdateFlashcardValidator: AbstractValidator<AddOrUpdateFlashcardDto>
{
    public AddOrUpdateFlashcardValidator(ICollectionRepository collectionRepository, IFlashcardRepository flashcardRepository)
    {
        RuleFor(f => f.CollectionId)
            .NotNull()
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("No collection Id provided");

        RuleFor(f => f.CollectionId)
            .MustAsync(async (id, cancellationToken) =>
                await collectionRepository.AnyAsync(c => c.Id == id, cancellationToken))
            .WithMessage($"Collection not found.");
        
        RuleFor(f => new { f.FlashcardId, f.CollectionId, f.WordOrPhrase, f.FlashcardBaseId })
            .MustAsync(async (data, cancellationToken) =>
                !await flashcardRepository.AnyAsync(f => 
                    ((!data.FlashcardId.HasValue || data.FlashcardId == Guid.Empty) 
                        && f.CollectionId == data.CollectionId 
                        && f.FlashcardBase.WordOrPhrase.ToLower() == data.WordOrPhrase.ToLower()) 
                    || (f.CollectionId == data.CollectionId 
                        && f.FlashcardBaseId != data.FlashcardBaseId
                        && f.FlashcardBase.WordOrPhrase.ToLower() == data.WordOrPhrase.ToLower()), 
                    cancellationToken))
            .WithMessage($"Flashcard with this word or phrase already exists in the collection.");
        
        RuleFor(f => f.WordOrPhrase)
            .NotNull()
            .NotEmpty()
            .WithMessage("No word or phrase provided.");

        RuleFor(f => f.Meanings)
            .NotNull()
            .NotEmpty()
            .WithMessage("Flashcard must contain at least one meaning.");

        RuleFor(f => f.Meanings)
            .Must((meanings) =>
                meanings.DistinctBy(f => f.Value).Count() == meanings.Count)
            .WithMessage("Conflict: Two or more meanings have the same value.");
    }
}