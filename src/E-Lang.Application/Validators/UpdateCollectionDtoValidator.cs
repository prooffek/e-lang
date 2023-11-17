using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using FluentValidation;

namespace E_Lang.Application.Validators;

public class UpdateCollectionDtoValidator : AbstractValidator<UpdateCollectionDto>
{
    public UpdateCollectionDtoValidator(ICollectionRepository collectionRepository, IUserService userService)
    {
        RuleFor(c => c.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("Collection id cannot be empty.");

        RuleFor(c => c.Id)
            .MustAsync(async (id, cancellationToken) =>
                await collectionRepository.AnyAsync(c => c.Id == id, cancellationToken))
            .WithMessage("Collection not found.");

        RuleFor(c => c.Name)
            .NotNull()
            .NotEmpty()
            .MinimumLength(Collection.NAME_MIN_LENGTH)
            .WithMessage($"Collection name must be at least {Collection.NAME_MIN_LENGTH} character long.");

        RuleFor(c => c.Name)
            .MaximumLength(Collection.NAME_MAX_LENGTH)
            .WithMessage($"Collection name cannot be longer than {Collection.NAME_MAX_LENGTH} characters");

        RuleFor(c => new { c.Id, c.ParentCollectionId })
            .Must((ids) => ids.Id != ids.ParentCollectionId)
            .WithMessage("Collection cannot be its own subcollection.");
        
        RuleFor(c => c.ParentCollectionId)
            .MustAsync(async (id, cancellationToken) =>
            {
                return !id.HasValue || id.Value == Guid.Empty ||
                       await collectionRepository.AnyAsync(c => c.Id == id, cancellationToken);
            }).WithMessage("Cannot assign to the collection, because it does not exist.");
    }
}