using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using FluentValidation;

namespace E_Lang.Application.Validators;

public class CreateCollectionDtoValidator : AbstractValidator<CreateCollectionDto>
{
    public CreateCollectionDtoValidator(ICollectionRepository collectionRepository, IUserService userService)
    {
        RuleFor(collection => collection.Name)
            .NotNull()
            .NotEmpty()
            .MinimumLength(Collection.NAME_MIN_LENGTH)
            .WithMessage($"Collection name cannot be null nor empty. It must be at least {Collection.NAME_MIN_LENGTH} character long.");

        RuleFor(collection => collection.Name)
            .MaximumLength(Collection.NAME_MAX_LENGTH)
            .WithMessage($"Collection name cannot be longer than {Collection.NAME_MAX_LENGTH} characters.");

        RuleFor(collection => collection.Name)
            .MustAsync(async (name, cancellationToken) =>
            {
                var user = await userService.GetCurrentUser(cancellationToken);
                
                return user != null && !await collectionRepository.IsNameAlreadyUsedAsync(user.Id,  name, cancellationToken);
            })
            .WithMessage("Collection with this name already exists.");

        RuleFor(c => c.ParentCollectionId)
            .MustAsync(async (id, cancellationToken) =>
            {
                return !id.HasValue || id.Value == Guid.Empty ||
                       await collectionRepository.AnyAsync(c => c.Id == id, cancellationToken);
            }).WithMessage("Cannot assign to the collection, because it does not exist.");
    }
}