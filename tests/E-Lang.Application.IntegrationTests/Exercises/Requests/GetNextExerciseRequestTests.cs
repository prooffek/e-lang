using E_Lang.Application.Common.Errors;
using E_Lang.Application.Exercises.Requests;
using Entities = E_Lang.Domain.Entities;
using E_Lang.Tests.Common.Mocks;
using FluentAssertions;
using E_Lang.Application.Common.Enums;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.IntegrationTests.Exercises.Requests
{
    [TestClass]
    public class GetNextExerciseRequestTests : Setup
    {
        private const int ALL_ANSWERS_NUMBER = 4;
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
        public async Task GetNextExerciseRequestHandler_Handle_ShouldThrowIfUserNotFound()
        {
            // Arrange
            MockUserService.CurrentUser = null;

            var request = new GetNextExerciseRequest();

            var expectedException = new UserNotFoundException();

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<UserNotFoundException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be(expectedException.Message);
            exception.StatusCode.Should().Be(expectedException.StatusCode);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldThrowIfAttemptNotFound()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var attemptId = Guid.NewGuid();

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attemptId
            };

            NotFoundValidationException expectedException = new (nameof(Entities.Attempt), nameof(Entities.Attempt.Id), attemptId.ToString());

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<NotFoundValidationException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be(expectedException.Message);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldThrowIfNotOwnedByUser()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddUser(out var user2)
                    .SetUsername("User 2")
                    .SetEmail("user2.test.pl")
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .Build()
                .AddCollection(out var collection, user.Id)
                    .AddAttempt(out var attempt, quizType)
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user2;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt.Id
            };

            UnauthorizedException expectedException = new(user.Id, ActionTypes.Get);

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be(expectedException.Message);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldThrowIfAllCompletedFlashcardStates()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddCompletedFlashcardState(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            var expectedException = new WrongStateException(FlashcardStatus.InProgress, FlashcardStatus.Learnt, nameof(FlashcardState));

            // Act
            var exception = await Assert.ThrowsExceptionAsync<WrongStateException>(async () => await _mediator.Send(request));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be(expectedException.Message);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnStaheIsCompleteIfAllFlashcardStatesComplete()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.Should().NotBeNull();
            result.IsStageComplete.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotThrowIfLastInProgressFlashcardStateAnsweredIncorrect()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = false
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            response.ExerciseDto.Should().NotBeNull();
            response.IsStageComplete.Should().BeNull();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotReturnNull()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            response.ExerciseDto.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldSelectFlashcardFromCurrentCollection()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .AddCollection(out var collection2, user.Id)
                    .AddAttempt(out var attempt2, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage2)
                            .AddFlashcard(out var flashcard6, collection2)
                                .AddFlashcardBase(out var flashcardBase6)
                                    .SetWordOrPhrase("Phrase 6")
                                    .AddMeaning(out var meaning11)
                                        .SetValue("Phrase 6 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning12)
                                        .SetValue("Phrase 6 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard7, collection2)
                                .AddFlashcardBase(out var flashcardBase7)
                                    .SetWordOrPhrase("Phrase 7")
                                    .AddMeaning(out var meaning13)
                                        .SetValue("Phrase 7 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin14)
                                        .SetValue("Phrase 7 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard8, collection2)
                                .AddFlashcardBase(out var flashcardBase8)
                                    .SetWordOrPhrase("Phrase 8")
                                    .AddMeaning(out var meaning15)
                                        .SetValue("Phrase 8 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning16)
                                        .SetValue("Phrase 8 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard9, collection2)
                                .AddFlashcardBase(out var flashcardBase9)
                                    .SetWordOrPhrase("Phrase 9")
                                    .AddMeaning(out var meaning17)
                                        .SetValue("Phrase 9 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin18)
                                        .SetValue("Phrase 9 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard10, collection2)
                                .AddFlashcardBase(out var flashcardBase10)
                                    .SetWordOrPhrase("Phrase 10")
                                    .AddMeaning(out var meaning19)
                                        .SetValue("Phrase 10 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin20)
                                        .SetValue("Phrase 10 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var id = response.ExerciseDto.FlashcardStateId;
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(id);
            flashcardState.Should().NotBeNull();
            flashcardState.Flashcard.Should().NotBeNull();
            flashcardState.Flashcard.CollectionId.Should().Be(collection1.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldSelectFlashcardFromCurrentAttempt()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .AddCollection(out var collection2, user.Id)
                    .AddAttempt(out var attempt2, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage2)
                            .AddFlashcard(out var flashcard6, collection2)
                                .AddFlashcardBase(out var flashcardBase6)
                                    .SetWordOrPhrase("Phrase 6")
                                    .AddMeaning(out var meaning11)
                                        .SetValue("Phrase 6 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning12)
                                        .SetValue("Phrase 6 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard7, collection2)
                                .AddFlashcardBase(out var flashcardBase7)
                                    .SetWordOrPhrase("Phrase 7")
                                    .AddMeaning(out var meaning13)
                                        .SetValue("Phrase 7 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin14)
                                        .SetValue("Phrase 7 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard8, collection2)
                                .AddFlashcardBase(out var flashcardBase8)
                                    .SetWordOrPhrase("Phrase 8")
                                    .AddMeaning(out var meaning15)
                                        .SetValue("Phrase 8 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning16)
                                        .SetValue("Phrase 8 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard9, collection2)
                                .AddFlashcardBase(out var flashcardBase9)
                                    .SetWordOrPhrase("Phrase 9")
                                    .AddMeaning(out var meaning17)
                                        .SetValue("Phrase 9 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin18)
                                        .SetValue("Phrase 9 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard10, collection2)
                                .AddFlashcardBase(out var flashcardBase10)
                                    .SetWordOrPhrase("Phrase 10")
                                    .AddMeaning(out var meaning19)
                                        .SetValue("Phrase 10 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin20)
                                        .SetValue("Phrase 10 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var attempt = await _attemptRepostory.GetByIdAsync(request.AttemptId);
            attempt.CurrentStage.Flashcards
                .Any(f => f.Id == response.ExerciseDto.FlashcardStateId)
                .Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotChangeFlashcardStateIfNoIdProvided()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var flashcardStates = await _flashcardStateRepository.GetAllAsync();
            flashcardStates.All(fs => !(fs is InitFlashcardState)).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldChangeInitFlashcardStateToInProgressFlashcardState(bool isAnswerCorrect)
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsMatch()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards?.FirstOrDefault()?.Id,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var flashcardStates = await _flashcardStateRepository.GetAllAsync();
            flashcardStates.Where(fs => fs is InProgressFlashcardState).Should().HaveCount(1);
            flashcardStates.Where(fs => fs is InitFlashcardState).Should().HaveCount(4);


            var flashcardState = flashcardStates.SingleOrDefault(fs => fs is InProgressFlashcardState);
            flashcardState.Should().NotBeNull();
            flashcardState.Id.Should().Be(request.FlashcardStateId.Value);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotChangeInProgressFlashcardStateIfWrongAnswer()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsMatch()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 2")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 2 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin4)
                                            .SetValue("Phrase 2 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState3, quizType)
                                .AddFlashcard(out var flashcard3, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase3)
                                        .SetWordOrPhrase("Phrase 3")
                                        .AddMeaning(out var meaning5)
                                            .SetValue("Phrase 3 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning6)
                                            .SetValue("Phrase 3 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState4, quizType)
                                .AddFlashcard(out var flashcard4, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase4)
                                        .SetWordOrPhrase("Phrase 4")
                                        .AddMeaning(out var meaning7)
                                            .SetValue("Phrase 4 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin8)
                                            .SetValue("Phrase 4 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState5, quizType)
                                .AddFlashcard(out var flashcard5, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase5)
                                        .SetWordOrPhrase("Phrase 5")
                                        .AddMeaning(out var meaning9)
                                            .SetValue("Phrase 5 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin10)
                                            .SetValue("Phrase 5 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId =flashcardState1.Id,
                IsAnswerCorrect = false
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var flashcardStates = await _flashcardStateRepository.GetAllAsync();
            flashcardStates.All(fs => (fs is InProgressFlashcardState)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldFillInQuizTypeData()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(result.ExerciseDto.FlashcardStateId);

            result.ExerciseDto.Should().NotBeNull();
            result.ExerciseDto.Instruction.Should().Be(quizType.Instruction);
            result.ExerciseDto.WordOrPhrase.Should().Be(flashcardState.Flashcard.FlashcardBase.WordOrPhrase);
            result.ExerciseDto.CorrectAnswers.Should().NotBeNull();
            result.ExerciseDto.IncorrectAnswers.Should().NotBeNull();
            result.ExerciseDto.IsSingleSelect.Should().Be(quizType.IsSelect && quizType.MaxAnswersToSelect == 1);
            result.ExerciseDto.IsMultiSelect.Should().Be(quizType.MaxAnswersToSelect > 1);
            result.ExerciseDto.IsSelectMissing.Should().Be(quizType.IsSelectMissing);
            result.ExerciseDto.IsMatch.Should().Be(quizType.IsMatch);
            result.ExerciseDto.IsArrange.Should().Be(quizType.IsArrange);
            result.ExerciseDto.IsInput.Should().Be(quizType.IsInput);
            result.ExerciseDto.IsFillInBlank.Should().Be(quizType.IsFillInBlank);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldHaveAppropriateAnswersNumber()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.CorrectAnswers.Count().Should().Be(quizType.MaxAnswersToSelect);
            result.ExerciseDto.IncorrectAnswers.Count().Should().Be(ALL_ANSWERS_NUMBER - quizType.MaxAnswersToSelect);

            var allAnswers = result.ExerciseDto.CorrectAnswers.Concat(result.ExerciseDto.IncorrectAnswers);
            allAnswers.Should().HaveCount(ALL_ANSWERS_NUMBER);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldHaveAppropriateCorrectAnswersIfSelectCorrectIsTrue()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(result.ExerciseDto.FlashcardStateId);
            var meanings = flashcardState.Flashcard.FlashcardBase.Meanings.Select(m => m.Value);

            result.ExerciseDto.CorrectAnswers.Should().HaveCount(1);
            result.ExerciseDto.CorrectAnswers.Any(a => meanings.Contains(a.Value)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldHaveAppropriateCorrectAnswersIfSelectCorrectIsFalse()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(result.ExerciseDto.FlashcardStateId);
            var meanings = flashcardState.Flashcard.FlashcardBase.Meanings.Select(m => m.Value);

            result.ExerciseDto.CorrectAnswers.Any(a => meanings.Contains(a.Value)).Should().BeFalse();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldHaveAppropriateIncorrectAnswersIfSelectCorrectIsTrue()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(result.ExerciseDto.FlashcardStateId);
            var meanings = flashcardState.Flashcard.FlashcardBase.Meanings.Select(m => m.Value);

            result.ExerciseDto.IncorrectAnswers.Should().HaveCount(3);
            result.ExerciseDto.IncorrectAnswers.Any(a => meanings.Contains(a.Value)).Should().BeFalse();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldHaveAppropriateIncorrectAnswersIfSelectCorrectIsFalse()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(result.ExerciseDto.FlashcardStateId);
            var meanings = flashcardState.Flashcard.FlashcardBase.Meanings.Select(m => m.Value);

            result.ExerciseDto.IncorrectAnswers.Any(a => meanings.Contains(a.Value)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnAnswersFromTheSameCollection()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            var allMeanings = collection1
                .Flashcards
                .SelectMany(f => f.FlashcardBase.Meanings.Select(m => m.Value));

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.CorrectAnswers.All(a => allMeanings.Contains(a.Value)).Should().BeTrue();
            result.ExerciseDto.IncorrectAnswers.All(a => allMeanings.Contains(a.Value)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_CorrectAndIncorrectAnswersShouldNotOverlap()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                            .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                        .Build()
                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = attemptStage1.Flashcards.FirstOrDefault().Id,
                IsAnswerCorrect = false
            };

            var allMeanings = collection1
                .Flashcards
                .SelectMany(f => f.FlashcardBase.Meanings.Select(m => m.Value));

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var correctAnswers = result.ExerciseDto.CorrectAnswers.Select(v => v.Value);
            var incorrectAnswers = result.ExerciseDto.IncorrectAnswers.Select(v => v.Value);

            correctAnswers
                .Any(v => incorrectAnswers.Contains(v))
                .Should().BeFalse();

            incorrectAnswers
                .Any(v => correctAnswers.Contains(v))
                .Should().BeFalse();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnExerciseForInitFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(expectedFlashcardState.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnExerciseForInProgressFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsMatch()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(expectedFlashcardState.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldSetShowAgainOnIfAnswerIsIncorrectForInitFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 2")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 2 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin4)
                                            .SetValue("Phrase 2 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState3, quizType)
                                .AddFlashcard(out var flashcard3, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase3)
                                        .SetWordOrPhrase("Phrase 3")
                                        .AddMeaning(out var meaning5)
                                            .SetValue("Phrase 3 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning6)
                                            .SetValue("Phrase 3 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState4, quizType)
                                .AddFlashcard(out var flashcard4, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase4)
                                        .SetWordOrPhrase("Phrase 4")
                                        .AddMeaning(out var meaning7)
                                            .SetValue("Phrase 4 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin8)
                                            .SetValue("Phrase 4 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState5, quizType)
                                .AddFlashcard(out var flashcard5, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase5)
                                        .SetWordOrPhrase("Phrase 5")
                                        .AddMeaning(out var meaning9)
                                            .SetValue("Phrase 5 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin10)
                                            .SetValue("Phrase 5 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var initFlashcardState = attemptStage1.Flashcards.FirstOrDefault(fs => fs.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = initFlashcardState.Id,
                IsAnswerCorrect = false
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(initFlashcardState.Id);
            flashcardState.Should().NotBeNull().And.BeOfType<InProgressFlashcardState>();

            var showAgainOn = ((InProgressFlashcardState)flashcardState).ShowAgainOn;
            showAgainOn.Should().NotBeNull().And.NotBe(DateTime.MinValue);
            (showAgainOn > _dateTimeProvider.UtcNow).Should().BeTrue();
            (showAgainOn <= _dateTimeProvider.UtcNow.AddMinutes(1)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldUpdateShowAgainOnIfAnswerIsIncorrectForInProgressFlashcardState()
        {
            // Arrange
            var oldShowAgainOn = _dateTimeProvider.UtcNow.AddMinutes(-5);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .SetShowAgainOn(oldShowAgainOn)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 2")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 2 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin4)
                                            .SetValue("Phrase 2 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState3, quizType)
                                .AddFlashcard(out var flashcard3, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase3)
                                        .SetWordOrPhrase("Phrase 3")
                                        .AddMeaning(out var meaning5)
                                            .SetValue("Phrase 3 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning6)
                                            .SetValue("Phrase 3 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState4, quizType)
                                .AddFlashcard(out var flashcard4, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase4)
                                        .SetWordOrPhrase("Phrase 4")
                                        .AddMeaning(out var meaning7)
                                            .SetValue("Phrase 4 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin8)
                                            .SetValue("Phrase 4 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState5, quizType)
                                .AddFlashcard(out var flashcard5, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase5)
                                        .SetWordOrPhrase("Phrase 5")
                                        .AddMeaning(out var meaning9)
                                            .SetValue("Phrase 5 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin10)
                                            .SetValue("Phrase 5 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var initFlashcardState = attemptStage1.Flashcards.FirstOrDefault(fs => fs.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = initFlashcardState.Id,
                IsAnswerCorrect = false
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(initFlashcardState.Id);
            flashcardState.Should().NotBeNull().And.BeOfType<InProgressFlashcardState>();

            var showAgainOn = ((InProgressFlashcardState)flashcardState).ShowAgainOn;
            showAgainOn.Should()
                .NotBeNull()
                .And.NotBe(DateTime.MinValue)
                .And.NotBe(oldShowAgainOn);

            (showAgainOn > _dateTimeProvider.UtcNow).Should().BeTrue();
            (showAgainOn <= _dateTimeProvider.UtcNow.AddMinutes(1)).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldUpdateShowAgainOnIfAnswerIsCorrectForInitFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 2")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 2 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin4)
                                            .SetValue("Phrase 2 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState3, quizType)
                                .AddFlashcard(out var flashcard3, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase3)
                                        .SetWordOrPhrase("Phrase 3")
                                        .AddMeaning(out var meaning5)
                                            .SetValue("Phrase 3 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning6)
                                            .SetValue("Phrase 3 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState4, quizType)
                                .AddFlashcard(out var flashcard4, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase4)
                                        .SetWordOrPhrase("Phrase 4")
                                        .AddMeaning(out var meaning7)
                                            .SetValue("Phrase 4 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin8)
                                            .SetValue("Phrase 4 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState5, quizType)
                                .AddFlashcard(out var flashcard5, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase5)
                                        .SetWordOrPhrase("Phrase 5")
                                        .AddMeaning(out var meaning9)
                                            .SetValue("Phrase 5 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meanin10)
                                            .SetValue("Phrase 5 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var initFlashcardState = attemptStage1.Flashcards.FirstOrDefault(fs => fs.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = initFlashcardState.Id,
                IsAnswerCorrect = true
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(initFlashcardState.Id);
            flashcardState.Should().NotBeNull().And.BeOfType<InProgressFlashcardState>();

            ((InProgressFlashcardState)flashcardState).ShowAgainOn.Should().BeNull();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldTakeFlashcardStateWithShowAgainEarlierThenNow(int minutes)
        {
            // Arrange
            var showAgainOn = _dateTimeProvider.UtcNow.AddMinutes(-minutes);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .SetShowAgainOn(showAgainOn)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var initFlashcardState = attemptStage1.Flashcards.FirstOrDefault(fs => fs.FlashcardId == flashcard2.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = initFlashcardState.Id,
                IsAnswerCorrect = false
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(expectedFlashcardState.Id);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(10)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldTakeAgainFlashcardStateWithShowAgainEarlierThenNow(int minutes)
        {
            // Arrange
            _now = new DateTime(2022, 10, 10, 10, 00, 00);
            var showAgainOn = _dateTimeProvider.UtcNow.AddMinutes(-minutes);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(false)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .SetShowAgainOn(showAgainOn)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard3, collection1)
                                .AddFlashcardBase(out var flashcardBase3)
                                    .SetWordOrPhrase("Phrase 3")
                                    .AddMeaning(out var meaning5)
                                        .SetValue("Phrase 3 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning6)
                                        .SetValue("Phrase 3 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard4, collection1)
                                .AddFlashcardBase(out var flashcardBase4)
                                    .SetWordOrPhrase("Phrase 4")
                                    .AddMeaning(out var meaning7)
                                        .SetValue("Phrase 4 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin8)
                                        .SetValue("Phrase 4 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard5, collection1)
                                .AddFlashcardBase(out var flashcardBase5)
                                    .SetWordOrPhrase("Phrase 5")
                                    .AddMeaning(out var meaning9)
                                        .SetValue("Phrase 5 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin10)
                                        .SetValue("Phrase 5 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = expectedFlashcardState.Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(expectedFlashcardState.Id);
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(20)]
        [DataRow(30)]
        [DataRow(40)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldTakeOtherFlashcardStateWithShowAgainLessThenMinuteFromNow(int seconds)
        {
            // Arrange
            var showAgainOn = _dateTimeProvider.UtcNow.AddSeconds(seconds);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .SetShowAgainOn(showAgainOn)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState.Id,
                IsAnswerCorrect = false
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard2.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(expectedFlashcardState.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldTakeOtherFlashcardStateIfPossible()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState.Id,
                IsAnswerCorrect = false
            };

            var expectedFlashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard2.Id);

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(expectedFlashcardState.Id);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-20)]
        [DataRow(-90)]
        [DataRow(20)]
        [DataRow(90)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnTheSameFlashcardStateIfNoOtherFlashcardStateToSelect(int seconds)
        {
            // Arrange
            var showAgainOn = _dateTimeProvider.UtcNow.AddSeconds(seconds);

            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsMatch()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .SetShowAgainOn(showAgainOn)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 2")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 2 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meanin4)
                                        .SetValue("Phrase 2 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState.Id,
                IsAnswerCorrect = false
            };

            // Act
            var result = await _mediator.Send(request);

            // Assert
            result.ExerciseDto.FlashcardStateId.Should().Be(flashcardState.Id);
        }
        
        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnExerciseForAnotherQuizType()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attempt1
                .CurrentStage
                .Flashcards
                .FirstOrDefault(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
            };

            // Act
            var response1 = await _mediator.Send(request);
            request.FlashcardStateId = response1.ExerciseDto.FlashcardStateId;
            request.IsAnswerCorrect = true;
            var response2 = await _mediator.Send(request);

            // Assert
            var result = await _flashcardStateRepository.GetByIdAsync(response2.ExerciseDto.FlashcardStateId);
            result.Should().BeOfType<InProgressFlashcardState>();
            var state = (InProgressFlashcardState)result;
            state.CurrentQuizTypeId.Should().Be(quizType2.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldSetNewCurrentQuizIfAnswerCorrectForInProgressFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning4)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .AddCompletedQuizType(quizType)
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<InProgressFlashcardState>();
            var inProgressFlashcardState = (InProgressFlashcardState)flashcardState;
            inProgressFlashcardState.CurrentQuizTypeId.Should().Be(quizType2.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotChangeCurrentQuizTypeIfAnswerIsIncorrectFoInProgressFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning4)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .AddCompletedQuizType(quizType)
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = false
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<InProgressFlashcardState>();
            var inProgressFlashcardState = (InProgressFlashcardState)flashcardState;
            inProgressFlashcardState.CurrentQuizTypeId.Should().Be(quizType.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotChangeCurrentQuizTypeIfAnswerIsIncorrectFoInitFlashcardState()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning2)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddInProgressFlashcardState(out var flashcardState2, quizType)
                                .AddFlashcard(out var flashcard2, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase2)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning3)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning4)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .AddCompletedQuizType(quizType)
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attempt1
                .CurrentStage
                .Flashcards
                .First(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState.Id,
                IsAnswerCorrect = false
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var result = await _flashcardStateRepository.GetByIdAsync(flashcardState.Id);
            result.Should().BeOfType<InProgressFlashcardState>();
            var inProgressFlashcardState = (InProgressFlashcardState)result;
            inProgressFlashcardState.CurrentQuizTypeId.Should().Be(quizType.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldChangeToCompltedFlashcardStateIfAllQuizzesCompleted()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType2)
                                .AddCompletedQuizType(quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<CompletedFlashcardState>();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldAddQuizToSeenQuizTypes(bool isANswerCorrect)
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = isANswerCorrect
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<InProgressFlashcardState>();
            var inProgressFlashcardState = (InProgressFlashcardState)flashcardState;
            inProgressFlashcardState.SeenQuizTypes.Should().HaveCount(1);
            var seenQuiz = inProgressFlashcardState.SeenQuizTypes.First();
            seenQuiz.QuizTypeId.Should().Be(quizType.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldAddQuizToCompletedQuizTypes()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<InProgressFlashcardState>();
            var inProgressFlashcardState = (InProgressFlashcardState)flashcardState;
            inProgressFlashcardState.CompletedQuizTypes.Should().HaveCount(1);
            var completedQuizId = inProgressFlashcardState.CompletedQuizTypes.First().QuizTypeId;
            completedQuizId.Should().Be(quizType.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotAddQuizToCompletedQuizTypes()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = false
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<InProgressFlashcardState>();
            var inProgressFlashcardState = (InProgressFlashcardState)flashcardState;
            inProgressFlashcardState.CompletedQuizTypes.Should().HaveCount(0);
        }

        [TestMethod]
        [DataRow(30)]
        [DataRow(50)]
        [DataRow(65)]
        [DataRow(79)]
        [DataRow(81)]
        [DataRow(97)]
        [DataRow(100)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldChangeToCompletedFlashcardStateIfAllQuizzesSeenAndEnoughQuizzesCompleted(int minCompleted)
        {
            // Arrange
            var builder = _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType1)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType3)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType4)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType5)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType6)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType7)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType8)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType9)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType10)
                    .SetIsArrange()
                    .Build();

            List<QuizType> quizzes = new()
            {
                quizType1, quizType2, quizType3, quizType4, quizType5, quizType6, quizType7, quizType8, quizType9
            };

            var take = (int)Math.Ceiling((decimal)minCompleted / 10);

            await builder
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType1)
                        .AddQuizTypes(quizzes)
                        .SetMinCompletedQuizzesPerCent(minCompleted)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType1)
                                .AddSeenQuizTypes(quizzes)
                                .AddCompletedQuizTypes(quizzes.Take(take))
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<CompletedFlashcardState>();
        }

        [TestMethod]
        [DataRow(30)]
        [DataRow(50)]
        [DataRow(65)]
        [DataRow(79)]
        [DataRow(81)]
        [DataRow(97)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldNotChangeToCompletedFlashcardStateIfAllQuizzesSeenButNotEnoughQuizzesCompleted(int minCompleted)
        {
            // Arrange
            var builder = _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType1)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType3)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType4)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType5)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType6)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType7)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType8)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType9)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType10)
                    .SetIsArrange()
                    .Build();

            List<QuizType> quizzes = new()
            {
                quizType1, quizType2, quizType3, quizType4, quizType5, quizType6, quizType7, quizType8, quizType9
            };

            var take = (int)Math.Floor((decimal)minCompleted / 10 - 1);

            await builder
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType1)
                        .AddQuizTypes(quizzes)
                        .SetMinCompletedQuizzesPerCent(minCompleted)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType10)
                                .AddSeenQuizTypes(quizzes)
                                .AddCompletedQuizType(quizType1)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = false
            };

            // Act
            var response1 = await _mediator.Send(request);

            // Assert
            var flashcardState = await _flashcardStateRepository.GetByIdAsync(flashcardState1.Id);
            flashcardState.Should().BeOfType<InProgressFlashcardState>();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldReturnStageCompletedIfNoMoreFlashcardsInAttempt()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType2)
                                .AddCompletedQuizType(quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            response.Should().NotBeNull();
            response.IsStageComplete.Should().BeTrue();
            response.ExerciseDto.Should().BeNull();
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldCompleteCurrentAttemptStage()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType2)
                                .AddCompletedQuizType(quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var completedAttemptStage = await _attemptStageRepostory.GetByIdAsync(attemptStage1.Id);
            completedAttemptStage.Stage.Should().Be(AttemptStageType.Complete);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldCreateNewAttemptStage()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType2)
                                .AddCompletedQuizType(quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .SetWordOrPhrase("Phrase 3")
                            .AddMeaning(out var meaning5)
                                .SetValue("Phrase 3 Meaning 1")
                                .Build()
                            .AddMeaning(out var meaning6)
                                .SetValue("Phrase 3 Meaning 2")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Phrase 4")
                            .AddMeaning(out var meaning7)
                                .SetValue("Phrase 4 Meaning 1")
                                .Build()
                            .AddMeaning(out var meaning8)
                                .SetValue("Phrase 4 Meaning 2")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var attemptStages = await _attemptStageRepostory.GetAllAsync();
            var newAttemptStage = attemptStages.FirstOrDefault(x => x.Id != attemptStage1.Id);

            newAttemptStage.Should().NotBeNull();
            newAttemptStage.Stage.Should().Be(AttemptStageType.Init);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldAssignNewAttemptStageToAttempt()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType2)
                                .AddCompletedQuizType(quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .SetWordOrPhrase("Phrase 3")
                            .AddMeaning(out var meaning5)
                                .SetValue("Phrase 3 Meaning 1")
                                .Build()
                            .AddMeaning(out var meaning6)
                                .SetValue("Phrase 3 Meaning 2")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Phrase 4")
                            .AddMeaning(out var meaning7)
                                .SetValue("Phrase 4 Meaning 1")
                                .Build()
                            .AddMeaning(out var meaning8)
                                .SetValue("Phrase 4 Meaning 2")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var attemptStages = await _attemptStageRepostory.GetAllAsync();
            var newAttemptStage = attemptStages.FirstOrDefault(x => x.Id != attemptStage1.Id);

            var resultAttempt = await _attemptRepostory.GetByIdAsync(attempt1.Id);
            resultAttempt.CurrentStage.Id.Should().NotBe(attemptStage1.Id)
                .And.Be(newAttemptStage.Id);
        }

        [TestMethod]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldAddFlashcardsOfCompletedAttemptToCompletedFlashcardsList()
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddInProgressFlashcardState(out var flashcardState1, quizType2)
                                .AddCompletedQuizType(quizType)
                                .AddFlashcard(out var flashcard1, collection1.Id, user.Id)
                                    .AddFlashcardBase(out var flashcardBase1)
                                        .SetWordOrPhrase("Phrase 1")
                                        .AddMeaning(out var meaning1)
                                            .SetValue("Phrase 1 Meaning 1")
                                            .Build()
                                        .AddMeaning(out var meaning2)
                                            .SetValue("Phrase 1 Meaning 2")
                                            .Build()
                                        .Build()
                                    .Build()
                                .Build()
                            .AddCompletedFlashcardState(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .AddMeaning(out var meaning4)
                                        .SetValue("Phrase 1 Meaning 2")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard3)
                        .AddFlashcardBase(out var flashcardBase3)
                            .SetWordOrPhrase("Phrase 3")
                            .AddMeaning(out var meaning5)
                                .SetValue("Phrase 3 Meaning 1")
                                .Build()
                            .AddMeaning(out var meaning6)
                                .SetValue("Phrase 3 Meaning 2")
                                .Build()
                            .Build()
                        .Build()
                    .AddFlashcard(out var flashcard4)
                        .AddFlashcardBase(out var flashcardBase4)
                            .SetWordOrPhrase("Phrase 4")
                            .AddMeaning(out var meaning7)
                                .SetValue("Phrase 4 Meaning 1")
                                .Build()
                            .AddMeaning(out var meaning8)
                                .SetValue("Phrase 4 Meaning 2")
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState1.Id,
                IsAnswerCorrect = true
            };

            var completedFlasgcardIds = attempt1
                .CurrentStage
                .Flashcards
                .Select(x => x.FlashcardId)
                .ToHashSet();

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var resultAttempt = await _attemptRepostory.GetByIdAsync(attempt1.Id);
            resultAttempt.CompletedFlashcards.Should().NotBeNullOrEmpty()
                .And.HaveCount(2);
            resultAttempt.CompletedFlashcards.All(x => completedFlasgcardIds.Contains(x.Id))
                .Should().BeTrue();
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldCompleteMultiselectQuizzesIfOnlyOnMeaning(int maxAnswersToSelect)
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType3)
                    .SetIsMultiselect(true, maxAnswersToSelect)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddQuizType(quizType3)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attemptStage1.Flashcards.First(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var resultFlashcardSate = await _flashcardStateRepository.GetByIdAsync(flashcardState.Id);
            resultFlashcardSate.Should().BeOfType<InProgressFlashcardState>();

            var result = (InProgressFlashcardState) resultFlashcardSate;
            result.CompletedQuizTypes.Should().NotBeNullOrEmpty();
            result.CompletedQuizTypes.Any(x => x.QuizTypeId == quizType3.Id).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public async Task GetNextExerciseRequestHandler_Handle_ShouldSetMultiselectQuizzesAsSeenIfOnlyOnMeaning(int maxAnswersToSelect)
        {
            // Arrange
            await _testBuilder
                .AddUser(out var user)
                    .Build()
                .AddQuizType(user.Id, out var quizType)
                    .SetIsFirst()
                    .SetIsSingleSelect(true)
                    .Build()
                .AddQuizType(user.Id, out var quizType2)
                    .SetIsArrange()
                    .Build()
                .AddQuizType(user.Id, out var quizType3)
                    .SetIsMultiselect(true, maxAnswersToSelect)
                    .Build()
                .AddCollection(out var collection1, user.Id)
                    .AddAttempt(out var attempt1, quizType)
                        .AddQuizType(quizType2)
                        .AddQuizType(quizType3)
                        .AddInitAttemptStageAsCurrentStage(out var attemptStage1)
                            .AddFlashcard(out var flashcard1, collection1)
                                .AddFlashcardBase(out var flashcardBase1)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning1)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .Build()
                                .Build()
                            .AddFlashcard(out var flashcard2, collection1)
                                .AddFlashcardBase(out var flashcardBase2)
                                    .SetWordOrPhrase("Phrase 1")
                                    .AddMeaning(out var meaning3)
                                        .SetValue("Phrase 1 Meaning 1")
                                        .Build()
                                    .Build()
                                .Build()
                            .Build()
                        .Build()
                    .Build()
                .SaveAsync();

            MockUserService.CurrentUser = user;

            var flashcardState = attemptStage1.Flashcards.First(x => x.FlashcardId == flashcard1.Id);

            var request = new GetNextExerciseRequest()
            {
                AttemptId = attempt1.Id,
                FlashcardStateId = flashcardState.Id,
                IsAnswerCorrect = true
            };

            // Act
            var response = await _mediator.Send(request);

            // Assert
            var resultFlashcardSate = await _flashcardStateRepository.GetByIdAsync(flashcardState.Id);
            resultFlashcardSate.Should().BeOfType<InProgressFlashcardState>();

            var result = (InProgressFlashcardState)resultFlashcardSate;
            result.SeenQuizTypes.Should().NotBeNullOrEmpty();
            result.SeenQuizTypes.Any(x => x.QuizTypeId == quizType3.Id).Should().BeTrue();
        }
    }
}
