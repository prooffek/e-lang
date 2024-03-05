using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Services
{
    [TestClass]
    public class AttemptStageServiceTests : Setup
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            InitClass();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            InitTest();
        }

        [TestCleanup]
        public void CleanUp()
        {
            CleanupTest();
        }

        [TestMethod]
        public async Task AttemptStageService_GetNextAttemptStage_ShouldExcludeCompletedFlashcards()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var firstQuiz)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddAttempt(out var attempt, firstQuiz)
                        .AddCompletedFlashcard(out var flashcard1)
                            .AddFlashcardBase(out var flashcardBase1)
                                .AddMeaning(out var meaning1)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard2)
                            .AddFlashcardBase(out var flashcardBase2)
                                .AddMeaning(out var meaning2)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard3)
                            .AddFlashcardBase(out var flashcardBase3)
                                .AddMeaning(out var meaning3)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard4)
                            .AddFlashcardBase(out var flashcardBase4)
                                .AddMeaning(out var meaning4)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard5)
                            .AddFlashcardBase(out var flashcardBase5)
                                .AddMeaning(out var meaning5)
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard6)
                        .AddFlashcardBase(out var flashcardBase6)
                            .AddMeaning(out var meaning6)
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard7)
                        .AddFlashcardBase(out var flashcardBase7)
                            .AddMeaning(out var meaning7)
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard8)
                        .AddFlashcardBase(out var flashcardBase8)
                            .AddMeaning(out var meaning8)
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard9)
                        .AddFlashcardBase(out var flashcardBase9)
                            .AddMeaning(out var meaning9)
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard10)
                        .AddFlashcardBase(out var flashcardBase10)
                            .AddMeaning(out var meaning10)
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            var completedFlashcardIds = attempt.CompletedFlashcards
                .Select(x => x.Id)
                .ToHashSet();

            // Act
            var result = await _attemptStageService.GetNextAttemptStage(attempt, default);

            // Assert
            result.Should().NotBeNull();

            var flashcardStates = result.Flashcards;
            flashcardStates.Any(x => completedFlashcardIds.Contains(x.FlashcardId.Value))
                .Should().BeFalse();
        }

        [TestMethod]
        public async Task AttemptStageService_GetNextAttemptStage_ShouldReturnNullIfAllFlashcardsCompleted()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var firstQuiz)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddAttempt(out var attempt, firstQuiz)
                        .AddCompletedFlashcard(out var flashcard1)
                            .AddFlashcardBase(out var flashcardBase1)
                                .AddMeaning(out var meaning1)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard2)
                            .AddFlashcardBase(out var flashcardBase2)
                                    .AddMeaning(out var meaning2)
                                        .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard3)
                            .AddFlashcardBase(out var flashcardBase3)
                                .AddMeaning(out var meaning3)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard4)
                            .AddFlashcardBase(out var flashcardBase4)
                                .AddMeaning(out var meaning4)
                                    .Build()
                                .Build()
                            .Build()
                        .AddCompletedFlashcard(out var flashcard5)
                            .AddFlashcardBase(out var flashcardBase5)
                                .AddMeaning(out var meaning5)
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            var completedFlashcardIds = attempt.CompletedFlashcards
                .Select(x => x.Id)
                .ToHashSet();

            // Act
            var result = await _attemptStageService.GetNextAttemptStage(attempt, default);

            // Assert
            result.Should().BeNull();
        }
    }
}
