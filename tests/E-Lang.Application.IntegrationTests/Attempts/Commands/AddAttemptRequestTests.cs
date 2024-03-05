using E_Lang.Application.Attempts.Commands;
using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Errors;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.IntegrationTests.Attempts.Commands
{
    [TestClass]
    public class AddAttemptRequestTests : Setup
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
        public async Task AddAttemptRequest_Handle_ShouldThrowIfUserNotFound()
        {
            // Arrange 
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = null;

            var dto = new AddAttemptDto();

            var request = new AddAttemptRequest
            {
                Attempt = dto
            };

            var expectedException = new UserNotFoundException();

            // Act
            var exception = await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () =>
                await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be(expectedException.Message);
            exception.StatusCode.Should().Be(expectedException.StatusCode);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task AddAttemptRequest_Handle_ShouldCreateAttempt(bool includeMeanings)
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .SetWordOrPhrase("Word 1")
                            .Build()
                        .AddMeaning(out var meaning1, flashcardBase1.Id)
                            .SetValue("Meaning 1")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard2)
                        .AddFlashcardBase(out var flashcardBase2)
                            .SetWordOrPhrase("Word 2")
                            .Build()
                        .AddMeaning(out var meaning2, flashcardBase1.Id)
                            .SetValue("Meaning 2")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .SetWordOrPhrase("Word 3")
                            .Build()
                        .AddMeaning(out var meaning3, flashcardBase3.Id)
                            .SetValue("Meaning 3")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Word 4")
                            .Build()
                        .AddMeaning(out var meaning4, flashcardBase4.Id)
                            .SetValue("Meaning 4")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .SetWordOrPhrase("Word 5")
                            .Build()
                        .AddMeaning(out var meaning5, flashcardBase5.Id)
                            .SetValue("Meaning 5")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard6)
                        .AddFlashcardBase(out var flashcardBase6)
                            .SetWordOrPhrase("Word 6")
                            .Build()
                        .AddMeaning(out var meaning6, flashcardBase6.Id)
                            .SetValue("Meaning 6")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard7)
                        .AddFlashcardBase(out var flashcardBase7)
                            .SetWordOrPhrase("Word 7")
                            .Build()
                        .AddMeaning(out var meaning7, flashcardBase7.Id)
                            .SetValue("Meaning 7")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard8)
                        .AddFlashcardBase(out var flashcardBase8)
                            .SetWordOrPhrase("Word 8")
                            .Build()
                        .AddMeaning(out var meaning8, flashcardBase8.Id)
                            .SetValue("Meaning 8")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard9)
                        .AddFlashcardBase(out var flashcardBase9)
                            .SetWordOrPhrase("Word 9")
                            .Build()
                        .AddMeaning(out var meaning9, flashcardBase9.Id)
                            .SetValue("Meaning 9")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard10)
                        .AddFlashcardBase(out var flashcardBase10)
                            .SetWordOrPhrase("Word 10")
                            .Build()
                        .AddMeaning(out var meaning10, flashcardBase10.Id)
                            .SetValue("Meaning 10")
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var addAttemptDto = new AddAttemptDto()
            {
                Name = "New attempt",
                CollectionId = collection.Id,
                MaxFlashcardsPerStage = 5,
                MaxQuizTypesPerFlashcard = 3,
                MinCompletedQuizzesPerCent = 100,
                Order = FlashcardOrder.Random,
                IncludeMeanings = includeMeanings
            };

            var request = new AddAttemptRequest()
            {
                Attempt = addAttemptDto
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            response.Should().NotBeNull();

            var attempt = await _attemptRepostory.GetByIdAsync(response.Id);
            attempt.Should().NotBeNull();
            attempt.Name.Should().Be(addAttemptDto.Name);
            attempt.MaxFlashcardsPerStage.Should().Be(addAttemptDto.MaxFlashcardsPerStage);
            attempt.MaxQuizTypesPerFlashcard.Should().Be(addAttemptDto.MaxQuizTypesPerFlashcard);
            attempt.MinCompletedQuizzesPerCent.Should().Be(addAttemptDto.MinCompletedQuizzesPerCent);
            attempt.Order.Should().Be(addAttemptDto.Order);
            attempt.IncludeMeanings.Should().Be(addAttemptDto.IncludeMeanings);
            attempt.Properties.Should().BeNullOrEmpty();
            attempt.QuizTypes.Should().BeNullOrEmpty();
            attempt.CollectionId.Should().Be(addAttemptDto.CollectionId);
            attempt.CompletedFlashcards.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public async Task AddAttemptRequest_Handle_ShouldCreateInitAttemptStage()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .SetWordOrPhrase("Word 1")
                            .Build()
                        .AddMeaning(out var meaning1, flashcardBase1.Id)
                            .SetValue("Meaning 1")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard2)
                        .AddFlashcardBase(out var flashcardBase2)
                            .SetWordOrPhrase("Word 2")
                            .Build()
                        .AddMeaning(out var meaning2, flashcardBase1.Id)
                            .SetValue("Meaning 2")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .SetWordOrPhrase("Word 3")
                            .Build()
                        .AddMeaning(out var meaning3, flashcardBase3.Id)
                            .SetValue("Meaning 3")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Word 4")
                            .Build()
                        .AddMeaning(out var meaning4, flashcardBase4.Id)
                            .SetValue("Meaning 4")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .SetWordOrPhrase("Word 5")
                            .Build()
                        .AddMeaning(out var meaning5, flashcardBase5.Id)
                            .SetValue("Meaning 5")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard6)
                        .AddFlashcardBase(out var flashcardBase6)
                            .SetWordOrPhrase("Word 6")
                            .Build()
                        .AddMeaning(out var meaning6, flashcardBase6.Id)
                            .SetValue("Meaning 6")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard7)
                        .AddFlashcardBase(out var flashcardBase7)
                            .SetWordOrPhrase("Word 7")
                            .Build()
                        .AddMeaning(out var meaning7, flashcardBase7.Id)
                            .SetValue("Meaning 7")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard8)
                        .AddFlashcardBase(out var flashcardBase8)
                            .SetWordOrPhrase("Word 8")
                            .Build()
                        .AddMeaning(out var meaning8, flashcardBase8.Id)
                            .SetValue("Meaning 8")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard9)
                        .AddFlashcardBase(out var flashcardBase9)
                            .SetWordOrPhrase("Word 9")
                            .Build()
                        .AddMeaning(out var meaning9, flashcardBase9.Id)
                            .SetValue("Meaning 9")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard10)
                        .AddFlashcardBase(out var flashcardBase10)
                            .SetWordOrPhrase("Word 10")
                            .Build()
                        .AddMeaning(out var meaning10, flashcardBase10.Id)
                            .SetValue("Meaning 10")
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var addAttemptDto = new AddAttemptDto()
            {
                Name = "New attempt",
                CollectionId = collection.Id,
                MaxFlashcardsPerStage = 5,
                MaxQuizTypesPerFlashcard = 3,
                MinCompletedQuizzesPerCent = 100,
                Order = FlashcardOrder.Random,
                IncludeMeanings = true
            };

            var request = new AddAttemptRequest()
            {
                Attempt = addAttemptDto
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            response.Should().NotBeNull();

            var attempt = await _attemptRepostory.GetByIdAsync(response.Id);
            attempt.Should().NotBeNull();

            var stage = attempt.CurrentStage;
            stage.Should().NotBeNull()
                .And.BeOfType<AttemptStage>();
            stage.Flashcards.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(20)]
        public async Task AddAttemptRequest_Handle_ShouldSelectCorrectNumberOfFlashcards(int maxFlashcards)
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddFlashcard(out var flashcard1)
                        .AddFlashcardBase(out var flashcardBase1)
                            .SetWordOrPhrase("Word 1")
                            .Build()
                        .AddMeaning(out var meaning1, flashcardBase1.Id)
                            .SetValue("Meaning 1")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard2)
                        .AddFlashcardBase(out var flashcardBase2)
                            .SetWordOrPhrase("Word 2")
                            .Build()
                        .AddMeaning(out var meaning2, flashcardBase1.Id)
                            .SetValue("Meaning 2")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .SetWordOrPhrase("Word 3")
                            .Build()
                        .AddMeaning(out var meaning3, flashcardBase3.Id)
                            .SetValue("Meaning 3")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Word 4")
                            .Build()
                        .AddMeaning(out var meaning4, flashcardBase4.Id)
                            .SetValue("Meaning 4")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard5)
                        .AddFlashcardBase(out var flashcardBase5)
                            .SetWordOrPhrase("Word 5")
                            .Build()
                        .AddMeaning(out var meaning5, flashcardBase5.Id)
                            .SetValue("Meaning 5")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard6)
                        .AddFlashcardBase(out var flashcardBase6)
                            .SetWordOrPhrase("Word 6")
                            .Build()
                        .AddMeaning(out var meaning6, flashcardBase6.Id)
                            .SetValue("Meaning 6")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard7)
                        .AddFlashcardBase(out var flashcardBase7)
                            .SetWordOrPhrase("Word 7")
                            .Build()
                        .AddMeaning(out var meaning7, flashcardBase7.Id)
                            .SetValue("Meaning 7")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard8)
                        .AddFlashcardBase(out var flashcardBase8)
                            .SetWordOrPhrase("Word 8")
                            .Build()
                        .AddMeaning(out var meaning8, flashcardBase8.Id)
                            .SetValue("Meaning 8")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard9)
                        .AddFlashcardBase(out var flashcardBase9)
                            .SetWordOrPhrase("Word 9")
                            .Build()
                        .AddMeaning(out var meaning9, flashcardBase9.Id)
                            .SetValue("Meaning 9")
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard10)
                        .AddFlashcardBase(out var flashcardBase10)
                            .SetWordOrPhrase("Word 10")
                            .Build()
                        .AddMeaning(out var meaning10, flashcardBase10.Id)
                            .SetValue("Meaning 10")
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;
            _now = new DateTime(2022, 10, 10, 10, 00, 00);

            var expectedNumberOfFlashcards = maxFlashcards > 10 ? 10 : maxFlashcards;

            var addAttemptDto = new AddAttemptDto()
            {
                Name = "New attempt",
                CollectionId = collection.Id,
                MaxFlashcardsPerStage = maxFlashcards,
                MaxQuizTypesPerFlashcard = 3,
                MinCompletedQuizzesPerCent = 100,
                Order = FlashcardOrder.Random,
                IncludeMeanings = true
            };

            var request = new AddAttemptRequest()
            {
                Attempt = addAttemptDto
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            response.Should().NotBeNull();

            var attempt = await _attemptRepostory.GetByIdAsync(response.Id);
            attempt.Should().NotBeNull();

            var stage = attempt.CurrentStage;
            stage.Should().NotBeNull()
                .And.BeOfType<AttemptStage>();
            stage.Flashcards.Should().NotBeNullOrEmpty()
                .And.HaveCount(expectedNumberOfFlashcards);
        }
    }
}
