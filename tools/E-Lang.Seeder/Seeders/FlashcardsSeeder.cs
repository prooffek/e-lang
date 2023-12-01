using E_Lang.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Seeder.Seeders;

public class FlashcardsSeeder : SeederBase
{
    private readonly IAppDbContext _context;

    public FlashcardsSeeder(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _context = _serviceProvider.GetRequiredService<IAppDbContext>();
    }

    public override async Task Seed()
    {
        await SeedFlashcards();
    }

    private async Task SeedFlashcards()
    {
        var collectionIds = _context.Collections
            .Select(x => x.Id)
            .ToList();

        var userId = _userService.GetUserId();

        foreach (var collectionId in collectionIds)
        {
            await Builder
                .AddFlashcard(out var flashcard1, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase1)
                        .SetWordOrPhrase($"Flashcard {flashcardBase1.Id.ToString()}")
                        .AddMeaning(out var meaning1, flashcard1.Id)
                            .SetValue($"Value {flashcardBase1.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard2, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase2)
                        .SetWordOrPhrase($"Flashcard {flashcardBase2.Id.ToString()}")
                        .AddMeaning(out var meaning2, flashcard2.Id)
                            .SetValue($"Value {flashcardBase2.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard3, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase3)
                        .SetWordOrPhrase($"Flashcard {flashcardBase3.Id.ToString()}")
                        .AddMeaning(out var meaning3, flashcard3.Id)
                            .SetValue($"Value {flashcardBase3.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard4, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase4)
                        .SetWordOrPhrase($"Flashcard {flashcardBase4.Id.ToString()}")
                        .AddMeaning(out var meaning4, flashcard4.Id)
                            .SetValue($"Value {flashcardBase4.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard5, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase5)
                        .SetWordOrPhrase($"Flashcard {flashcardBase5.Id.ToString()}")
                        .AddMeaning(out var meaning5, flashcard5.Id)
                            .SetValue($"Value {flashcardBase5.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard6, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase6)
                        .SetWordOrPhrase($"Flashcard {flashcardBase6.Id.ToString()}")
                        .AddMeaning(out var meaning6, flashcard6.Id)
                            .SetValue($"Value {flashcardBase6.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard7, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase7)
                        .SetWordOrPhrase($"Flashcard {flashcardBase7.Id.ToString()}")
                        .AddMeaning(out var meaning7, flashcard7.Id)
                            .SetValue($"Value {flashcardBase7.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .AddFlashcard(out var flashcard8, collectionId, userId)
                    .AddFlashcardBase(out var flashcardBase8)
                        .SetWordOrPhrase($"Flashcard {flashcardBase8.Id.ToString()}")
                        .AddMeaning(out var meaning8, flashcard8.Id)
                            .SetValue($"Value {flashcardBase8.Id.ToString()}")
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();
        }
    }
}