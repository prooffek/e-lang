using E_Lang.Application.Services;
using E_Lang.Builder;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;

namespace E_Lang.Application.UnitTests.Services
{
    [TestClass]
    public class AttemptStageServiceTests : Setup
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
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
        public void AttemptStageService_GetInitAttemptStage_ShouldSetInitStage()
        {
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.AlphabeticalAsc;
            var service = new AttemptStageService();
            var maxFlashcardsNumber = 5;

            // Act
            var result = service.GetInitAttemptStage(flashcards, order, maxFlashcardsNumber);

            // Assert
            result.Should().NotBeNull();
            result.Stage.Should().Be(AttemptStageType.Init);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(10)]
        [DataRow(20)]
        public void AttemptStageService_GetInitAttemptStage_ShouldReturnCorrectNumberOfFlashcardStates(int maxFlashcardsNumber)
        {
            // Arrange
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.AlphabeticalAsc;
            var service = new AttemptStageService();
            var expectedNumber = maxFlashcardsNumber > flashcards.Count() 
                ? flashcards.Count() 
                : maxFlashcardsNumber;

            // Act
            var result = service.GetInitAttemptStage(flashcards, order, maxFlashcardsNumber);

            // Assert
            result.Should().NotBeNull();
            result.Flashcards.Should().NotBeNullOrEmpty()
                .And.HaveCount(expectedNumber);

        }

        [TestMethod]
        public void AttemptStageService_GetInitAttemptStage_ShouldOrderDescendingByWordOrPhrase()
        {
            // Arrange
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.AlphabeticalDesc;
            var service = new AttemptStageService();
            var maxFlashcards = 5;

            var expectedResult = flashcards
                .OrderByDescending(e => e.FlashcardBase.WordOrPhrase)
                .Take(maxFlashcards)
                .Select(f => (FlashcardState) new InitFlashcardState(f))
                .ToList();

            // Act
            var result = service.GetInitAttemptStage(flashcards, order, maxFlashcards);

            // Assert
            result.Should().NotBeNull();

            var resultFlashcards = result.Flashcards.ToList();
            resultFlashcards.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void AttemptStageService_GetInitAttemptStage_ShouldOrderAscendingByWordOrPhrase()
        {
            // Arrange
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.AlphabeticalAsc;
            var service = new AttemptStageService();
            var maxFlashcards = 5;

            var expectedResult = flashcards
                .OrderBy(e => e.FlashcardBase.WordOrPhrase)
                .Take(maxFlashcards)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();

            // Act
            var result = service.GetInitAttemptStage(flashcards, order, maxFlashcards);

            // Assert
            result.Should().NotBeNull();

            var resultFlashcards = result.Flashcards.ToList();
            resultFlashcards.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void AttemptStageService_GetInitAttemptStage_ShouldOrderDescendingByCreatedOn()
        {
            // Arrange
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.CreationDateDesc;
            var service = new AttemptStageService();
            var maxFlashcards = 5;

            var expectedResult = flashcards
                .OrderByDescending(e => e.CreatedOn)
                .Take(maxFlashcards)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();

            // Act
            var result = service.GetInitAttemptStage(flashcards, order, maxFlashcards);

            // Assert
            result.Should().NotBeNull();

            var resultFlashcards = result.Flashcards.ToList();
            resultFlashcards.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void AttemptStageService_GetInitAttemptStage_ShouldOrderAscendingByCreatedOn()
        {
            // Arrange
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.CreationDateAsc;
            var service = new AttemptStageService();
            var maxFlashcards = 5;

            var expectedResult = flashcards
                .OrderBy(e => e.CreatedOn)
                .Take(maxFlashcards)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();

            // Act
            var result = service.GetInitAttemptStage(flashcards, order, maxFlashcards);

            // Assert
            result.Should().NotBeNull();

            var resultFlashcards = result.Flashcards.ToList();
            resultFlashcards.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void AttemptStageService_GetInitAttemptStage_ShouldRandomiseFlashcardsOrder()
        {
            // Arrange
            var flashcards = GetFlashcards(10);
            var order = FlashcardOrder.Random;
            var service = new AttemptStageService();
            var maxFlashcards = 5;

            var orderByCreatedOnAsc = flashcards
                .OrderBy(e => e.CreatedOn)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();
            var orderByCreatedOnDesc = flashcards
                .OrderByDescending(e => e.CreatedOn)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();
            var orderByWordOrPhraseDesc = flashcards
                .OrderByDescending(e => e.FlashcardBase.WordOrPhrase)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();
            var orderByWordOrPhraseAsc = flashcards
                .OrderBy(e => e.FlashcardBase.WordOrPhrase)
                .Select(f => (FlashcardState)new InitFlashcardState(f))
                .ToList();

            // Act
            var result1 = service.GetInitAttemptStage(flashcards, order, maxFlashcards);
            var result2 = service.GetInitAttemptStage(flashcards, order, maxFlashcards);

            // Assert
            result1.Should().NotBeNull();

            var resultFlashcards = result1.Flashcards.ToList();
            resultFlashcards.Should().NotBeNullOrEmpty();

            resultFlashcards.Should().NotBeEquivalentTo(orderByCreatedOnAsc)
                .And.NotBeEquivalentTo(orderByCreatedOnDesc)
                .And.NotBeEquivalentTo(orderByWordOrPhraseAsc)
                .And.NotBeEquivalentTo(orderByWordOrPhraseDesc)
                .And.NotBeEquivalentTo(flashcards.Take(maxFlashcards))
                .And.NotBeEquivalentTo(result2.Flashcards);

        }

        private IEnumerable<Flashcard> GetFlashcards(int counter = 10)
        {
            var user = Entities.GetUser();
            var collection = Entities.GetCollection(user.Id);
            var dateTimeProvider = MockDateTimeProvider.GetInstance();
            MockDateTimeProvider.MockNow = new DateTime(2022, 10, 10, 10, 00, 00);

            var flashcards = new List<Flashcard>();

            for (int i = 0; i < counter; i++)
            {
                var now = dateTimeProvider.Now.AddDays(i % 3);
                var flashcard = GetFlashcard(collection);
                flashcard.CreatedOn = now;
                flashcard.ModifiedOn = now;
                flashcards.Add(flashcard);
            }

            return flashcards;
        }

        private Flashcard GetFlashcard(Collection collection)
        {
            var flashcardBase = Entities.GetFlashcardBase();
            flashcardBase.WordOrPhrase = Guid.NewGuid().ToString();
            var meaning = Entities.GetMeaning(flashcardBase.Id);
            meaning.FlashcardBase = flashcardBase;
            flashcardBase.Meanings.Add(meaning);
            var flashcard = Entities.GetFlashcard(collection.Id, collection.OwnerId);
            flashcard.Collection = collection;
            flashcard.FlashcardBase = flashcardBase;
            flashcard.FlashcardBaseId = flashcardBase.Id;

            return flashcard;
        }
    }
}
