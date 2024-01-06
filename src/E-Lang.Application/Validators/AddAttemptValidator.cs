using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using FluentValidation;

namespace E_Lang.Application.Validators
{
    public class AddAttemptValidator : AbstractValidator<AddAttemptDto>
    {
        public AddAttemptValidator(IUserService userService, ICollectionRepository collectionRepository,
            IAttemptRepository attemptRepository)
        {
            RuleFor(e => e.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Attempt name cannot be null or empty");

            RuleFor(e => new { Name = e.Name, CollectionId = e.CollectionId })
                .MustAsync(async (data, cancellationToken) =>
                    !await attemptRepository.AnyAsync(a => a.CollectionId == data.CollectionId && a.Name == data.Name))
                .WithMessage("Attempt with the name already exists.");


            RuleFor(e => e.CollectionId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Collection is required.")
                .MustAsync(async (collectionId, cancellationToken) =>
                {
                    var user = await userService.GetCurrentUser(cancellationToken);

                    return user != null && await collectionRepository.AnyAsync(c => c.Id == collectionId && c.OwnerId == user.Id, cancellationToken);
                })
                .WithMessage("Collection not found.");

            RuleFor(e => e.MaxFlashcardsPerStage)
                .NotNull()
                .WithMessage("Number of flashcards is required.")
                .GreaterThanOrEqualTo(1)
                .WithMessage("There must be at least 1 flashcard per stage.");

            RuleFor(e => new { collectionId = e.CollectionId, flashcardsNumber = e.MaxFlashcardsPerStage })
                .MustAsync(async (data, cancellationToken) =>
                {
                    var collection = await collectionRepository.GetWithExtensionsByIdAsync(data.collectionId, cancellationToken);
                    var flashcardsInCollection = collection?.Flashcards.Count ?? 0;

                    return flashcardsInCollection >= data.flashcardsNumber;
                })
                .WithMessage("The numer of flashcards in the stage cannot be greater than the number of flashcards in the collection.");

            RuleFor(e => e.MaxQuizTypesPerFlashcard)
                .NotNull()
                .WithMessage("Number of quizzes is required..")
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(10)
                .WithMessage("The number of quizzes per flashcard should be no less than 1 but no greater than 10.");

            RuleFor(e => e.MinCompletedQuizzesPerCent)
                .NotNull()
                .WithMessage("Lowest result is required..")
                .GreaterThanOrEqualTo(1)
                .WithMessage("You must pass at least 1% of the quizzes.")
                .LessThanOrEqualTo(100)
                .WithMessage("The result cannot be greater then 100%.");

            RuleFor(e => e.Order)
                .NotNull()
                .WithMessage("Flashcards selection rule is required.");

            RuleFor(e => e.IncludeMeanings)
                .NotNull()
                .WithMessage("Specify if quizzes for meanings should be included.");
        }
    }
}
