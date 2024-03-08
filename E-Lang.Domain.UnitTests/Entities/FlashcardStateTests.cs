using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using E_Lang.Domain.Interfaces;
using E_Lang.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace E_Lang.Domain.UnitTests.Entities
{
    [TestClass]
    public class FlashcardStateTests : Setup
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

        #region InitFlashcardState

        [TestMethod]
        [DataRow(null)]
        [DataRow(false)]
        [DataRow(true)]
        public void InitFlashcardState_GetNextState_ShouldReturnInProgressFlashcardSate(bool? isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<InProgressFlashcardState>();
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldLeaveListsEmpty()
        {
            // Arrange
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CompletedQuizTypes.Should().NotBeNull().And.BeEmpty();
            state.SeenQuizTypes.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldHaveCurrentQuizTypeNull()
        {
            // Arrange
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CurrentQuizTypeId.Should().BeNull();
            state.CurrentQuizType.Should().BeNull();
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldSetSeeAgainOnIfAnswerIncorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = false
            };

            var expectedShowAgainOn = data.UtcNow.AddMinutes(1);

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.ShowAgainOn.Should().NotBeNull()
                .And.Be(expectedShowAgainOn);
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldNotSetSeeAgainOnIfAnswerCorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.ShowAgainOn.Should().BeNull();
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void InitFlashcardState_GetNextState_ShouldSetQuizAsSeen(bool isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.SeenQuizTypes.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            state.SeenQuizTypes.Any(x => x.QuizTypeId == quiz1.Id).Should().BeTrue();
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldSetQuizAsCompleted()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.SeenQuizTypes.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            state.SeenQuizTypes.Any(x => x.QuizTypeId == quiz1.Id).Should().BeTrue();
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldNotSetCurrentQuiz()
        {
            // Arrange
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CurrentQuizTypeId.Should().BeNull();
            state.CurrentQuizType.Should().BeNull();
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldSetFirstQuiz()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = false
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CurrentQuizTypeId.Should().Be(quiz1.Id);
            state.CurrentQuizType.Should().BeEquivalentTo(quiz1);
        }

        [TestMethod]
        public void InitFlashcardState_GetNextState_ShouldSetNextQuiz()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CurrentQuizTypeId.Should().Be(quiz2.Id);
            state.CurrentQuizType.Should().BeEquivalentTo(quiz2);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(false)]
        [DataRow(true)]
        public void InitFlashcardState_GetNextState_ShouldSetLastSeenOn(bool? isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.Flashcard.LastSeenOn.Should().Be(data.UtcNow);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(false)]
        [DataRow(true)]
        public void InitFlashcardState_GetNextState_ShouldSetNewStatus(bool? isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.Flashcard.LastStatusChangedOn.Should().Be(data.UtcNow);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void InitFlashcardState_GetNextState_ShouldSetCurrentQuizToNullQuizNotFound(bool isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            var flashcardState = GetFlashcardState<InitFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            var result = flashcardState.GetNextState(data);

            // Assert
            result.Should().BeOfType<InProgressFlashcardState>();

            var resultFlashcardState = (InProgressFlashcardState) result;
            resultFlashcardState.CurrentQuizTypeId.Should().BeNull();
            resultFlashcardState.CurrentQuizType.Should().BeNull();
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void
        InitFlashcardState_GetNextState_ShouldCompleteMultiselectQuizzesIfOnlyOneMeaning(int maxAnswersCount)
        {
            // Arrange
            var quizzes = GetQuizzes(3).ToList();
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            secondQuiz.MaxAnswersToSelect = maxAnswersCount;
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = GetFlashcardState<InitFlashcardState>();
            flashcardState?.Flashcard?.FlashcardBase?.Meanings.Add(Builder.Entities.GetMeaning(flashcardState.Flashcard.FlashcardBaseId));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState?.GetNextState(data);

            // Assert
            result.Should().NotBeNull()
                .And.BeOfType<InProgressFlashcardState>();

            var inProgressFlashcardState = (InProgressFlashcardState) result;
            inProgressFlashcardState.CompletedQuizTypes.Should().NotBeNullOrEmpty();
            inProgressFlashcardState.CompletedQuizTypes.Any(x => x.QuizTypeId == secondQuiz.Id);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void
            InitFlashcardState_GetNextState_ShouldSetMultiselectQuizzesAsSeenIfOnlyOneMeaning(int maxAnswersCount)
        {
            // Arrange
            var quizzes = GetQuizzes(3).ToList();
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            secondQuiz.MaxAnswersToSelect = maxAnswersCount;
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = GetFlashcardState<InitFlashcardState>();
            flashcardState?.Flashcard?.FlashcardBase?.Meanings.Add(Builder.Entities.GetMeaning(flashcardState.Flashcard.FlashcardBaseId));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState?.GetNextState(data);

            // Assert
            result.Should().NotBeNull()
                .And.BeOfType<InProgressFlashcardState>();

            var inProgressFlashcardState = (InProgressFlashcardState)result;
            inProgressFlashcardState.SeenQuizTypes.Should().NotBeNullOrEmpty();
            inProgressFlashcardState.SeenQuizTypes.Any(x => x.QuizTypeId == secondQuiz.Id);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void
            InitFlashcardState_GetNextState_ShouldAllowMultiselectExerciseWithSelectionOfIncorrectAnswers(int maxAnswersCount)
        {
            // Arrange
            var quizzes = GetQuizzes(3).ToList();
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            secondQuiz.MaxAnswersToSelect = maxAnswersCount;
            secondQuiz.IsSelectCorrect = false;
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = GetFlashcardState<InitFlashcardState>();
            flashcardState?.Flashcard?.FlashcardBase?.Meanings.Add(Builder.Entities.GetMeaning(flashcardState.Flashcard.FlashcardBaseId));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState?.GetNextState(data);

            // Assert
            result.Should().NotBeNull()
                .And.BeOfType<InProgressFlashcardState>();

            var inProgressFlashcardState = (InProgressFlashcardState)result;
            inProgressFlashcardState.CompletedQuizTypes.FirstOrDefault(x => x.QuizTypeId == secondQuiz.Id).Should().BeNull();
            inProgressFlashcardState.SeenQuizTypes.FirstOrDefault(x => x.QuizTypeId == secondQuiz.Id).Should().BeNull();
        }

        #endregion

        #region InProgressFlashcardState

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldThrowIfAnswerNull()
        {
            // Arrange
            var flashcardState = GetFlashcardState<InProgressFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
            };

            // Act
            var exception = Assert.ThrowsException<NullReferenceException>(() => flashcardState.GetNextState(data));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("A flashcard in progress needs answer to get to the next stage.");
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldSetShowAgainOnIfAnswerIsIncorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InProgressFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = false
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.ShowAgainOn.Should().Be(data.UtcNow.AddMinutes(1));
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldHaveNullShowAgainOnIfAnswerIsCorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InProgressFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.ShowAgainOn.Should().BeNull();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InProgressFlashcardState_GetNextState_ShouldSetFlashcardLastSeenOn(bool isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = GetFlashcardState<InProgressFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.Flashcard.LastSeenOn.Should().Be(data.UtcNow);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InProgressFlashcardState_GetNextState_ShouldSetQuizAsSeen(bool isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = (InProgressFlashcardState) GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizType = quiz1;
            flashcardState.CurrentQuizTypeId = quiz1.Id;
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.SeenQuizTypes.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            state.SeenQuizTypes.Any(x => x.QuizTypeId == quiz1.Id).Should().BeTrue();
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldSetQuizAsCompletedIfAnswerCorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizType = quiz1;
            flashcardState.CurrentQuizTypeId = quiz1.Id;
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CompletedQuizTypes.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            state.CompletedQuizTypes.Any(x => x.QuizTypeId == quiz1.Id).Should().BeTrue();
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldNotSetQuizAsCompletedIfAnswerIncorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizType = quiz1;
            flashcardState.CurrentQuizTypeId = quiz1.Id;
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = false
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CompletedQuizTypes.Should().BeNullOrEmpty();
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(20)]
        [DataRow(30)]
        [DataRow(40)]
        [DataRow(50)]
        [DataRow(60)]
        [DataRow(70)]
        [DataRow(80)]
        [DataRow(90)]
        [DataRow(100)]
        public void InProgressFlashcardState_GetNextState_ShouldReturnCompletedFlashcardStateIfRequironmentFullfilled(int minResult)
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var lastQuiz = quizzes.Last();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = minResult;
            AddQuizTypes(attempt, quizzes);
            int take = minResult / 10 - 1;

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            var seenQuizzes = quizzes.Select(x => new SeenQuizType(flashcardState.Id, x.Id)).ToList();
            var completedQuizzes = quizzes.Take(take).Select(x => new CompletedQuizType(flashcardState.Id, x.Id)).ToList();
            flashcardState.CurrentQuizType = lastQuiz;
            flashcardState.CurrentQuizTypeId = lastQuiz.Id;
            flashcardState.SeenQuizTypes = seenQuizzes;
            flashcardState.CompletedQuizTypes = completedQuizzes;

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            result.Should().BeOfType<CompletedFlashcardState>();
        }

        [TestMethod]
        [DataRow(30)]
        [DataRow(40)]
        [DataRow(50)]
        [DataRow(60)]
        [DataRow(70)]
        [DataRow(80)]
        [DataRow(90)]
        [DataRow(100)]
        public void InProgressFlashcardState_GetNextState_ShouldReturnInProgressFlashcardStateIfRequironmentNotMet(int minResult)
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var lastQuiz = quizzes.Last();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = minResult;
            AddQuizTypes(attempt, quizzes);
            int take = minResult / 10 - 3;

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            var seenQuizzes = quizzes.Take(take + 1).Select(x => new SeenQuizType(flashcardState.Id, x.Id)).ToList();
            var completedQuizzes = quizzes.Take(take).Select(x => new CompletedQuizType(flashcardState.Id, x.Id)).ToList();
            flashcardState.CurrentQuizType = lastQuiz;
            flashcardState.CurrentQuizTypeId = lastQuiz.Id;
            flashcardState.SeenQuizTypes = seenQuizzes;
            flashcardState.CompletedQuizTypes = completedQuizzes;

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            result.Should().BeOfType<InProgressFlashcardState>();
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldSetNewCurrentQuizTypeIfAnswerCorrect()
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var quiz1 = Builder.Entities.GetQuizType();
            quiz1.IsFirst = true;
            var quiz2 = Builder.Entities.GetQuizType();
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizType(attempt, quiz1);
            AddQuizType(attempt, quiz2);
            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizType = quiz1;
            flashcardState.CurrentQuizTypeId = quiz1.Id;
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = flashcardState.GetNextState(data);

            // Assert
            var state = (InProgressFlashcardState)result;
            state.CurrentQuizTypeId.Should().Be(quiz2.Id);
            state.CurrentQuizType.Should().BeEquivalentTo(quiz2);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldReturnFirstQuizIfNotCompleted()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var expectedQuiz = quizzes.First(x => x.IsFirst);

            // Act
            var result = (InProgressFlashcardState)flashcardState.GetNextState(data);

            // Assert
            result.CurrentQuizTypeId.Should().Be(expectedQuiz.Id);
            result.CurrentQuizType.Should().BeEquivalentTo(expectedQuiz);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldReturnDefaultQuizIfFirstCompleted()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var defaultQuizzes = quizzes.Where(x => !x.IsFirst && x.IsDefault);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.SeenQuizTypes.Add(new SeenQuizType(flashcardState.Id, firstQuiz.Id));
            flashcardState.CompletedQuizTypes.Add(new CompletedQuizType(flashcardState.Id, firstQuiz.Id));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var defaultQuizzeIds = defaultQuizzes.Select(x => x.Id).ToHashSet();

            // Act
            var result = (InProgressFlashcardState)flashcardState.GetNextState(data);

            // Assert
            result.CurrentQuizTypeId.Should().NotBeNull();
            defaultQuizzeIds.Should().Contain(result.CurrentQuizTypeId!.Value);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldReturnNextQuizIfAllDefaultQuizesCompleted()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var defaultQuizzes = quizzes.Where(x => x.IsDefault);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.SeenQuizTypes = defaultQuizzes.Select(x => (new SeenQuizType(flashcardState.Id, x.Id))).ToList();
            flashcardState.CompletedQuizTypes = defaultQuizzes.Select(x => (new CompletedQuizType(flashcardState.Id, x.Id))).ToList();

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var defaultQuizzeIds = quizzes.Except(defaultQuizzes).Select(x => x.Id).ToHashSet();

            // Act
            var result = (InProgressFlashcardState)flashcardState.GetNextState(data);

            // Assert
            result.CurrentQuizTypeId.Should().NotBeNull();
            defaultQuizzeIds.Should().Contain(result.CurrentQuizTypeId!.Value);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldReturnNotCompletedQuiz()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);
            var take = quizzes.Count - 1;
            var completedQuizzes = quizzes.Take(take).ToList();

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.SeenQuizTypes = completedQuizzes.Select(x => (new SeenQuizType(flashcardState.Id, x.Id))).ToList();
            flashcardState.CompletedQuizTypes = completedQuizzes.Select(x => (new CompletedQuizType(flashcardState.Id, x.Id))).ToList();

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var expectedQuiz = quizzes.Except(completedQuizzes).First();

            // Act
            var result = (InProgressFlashcardState)flashcardState.GetNextState(data);

            // Assert
            result.CurrentQuizTypeId.Should().Be(expectedQuiz.Id);
            result.CurrentQuizType.Should().BeEquivalentTo(expectedQuiz);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldReturnDifferentQuiz()
        {
            // Arrange
            var quizzes = GetQuizzes(3).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            var lastQuiz = quizzes.First(x => !x.IsFirst && x.Id != secondQuiz.Id);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);
            var take = quizzes.Count - 1;
            var completedQuizzes = quizzes.Take(take).ToList();

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizTypeId = secondQuiz.Id;
            flashcardState.CurrentQuizType = secondQuiz;
            flashcardState.SeenQuizTypes.Add(new SeenQuizType(flashcardState.Id, firstQuiz.Id));
            flashcardState.CompletedQuizTypes.Add(new CompletedQuizType(flashcardState.Id, firstQuiz.Id));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            // Act
            var result = (InProgressFlashcardState)flashcardState.GetNextState(data);

            // Assert
            result.CurrentQuizTypeId.Should().Be(lastQuiz.Id);
            result.CurrentQuizType.Should().Be(lastQuiz);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetNextState_ShouldReturnTheSameQuizIfLast()
        {
            // Arrange
            var quizzes = GetQuizzes(2).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);
            var take = quizzes.Count - 1;
            var completedQuizzes = quizzes.Take(take).ToList();

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizTypeId = secondQuiz.Id;
            flashcardState.CurrentQuizType = secondQuiz;
            flashcardState.SeenQuizTypes.Add(new SeenQuizType(flashcardState.Id, firstQuiz.Id));
            flashcardState.CompletedQuizTypes.Add(new CompletedQuizType(flashcardState.Id, firstQuiz.Id));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = false
            };

            // Act
            var result = (InProgressFlashcardState)flashcardState.GetNextState(data);

            // Assert
            result.CurrentQuizTypeId.Should().Be(secondQuiz.Id);
            result.CurrentQuizType.Should().Be(secondQuiz);
        }

        #endregion

        #region CompletedFlashcardState

        [TestMethod]
        [DataRow(null)]
        [DataRow(false)]
        [DataRow(true)]
        public void CompletedFlashcardState_GetNextState_ShouldThrow(bool? isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            var flashcardState = GetFlashcardState<CompletedFlashcardState>();
            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = isAnswerCorrect
            };

            var exception = Assert.ThrowsException<Exception>(() => flashcardState.GetNextState(data));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Flashcard is already completed");

        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(false)]
        [DataRow(true)]
        public void CompletedFlashcardState_GetQuiz_ShouldThrow(bool? isAnswerCorrect)
        {
            // Arrange
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            var flashcardState = GetFlashcardState<CompletedFlashcardState>();

            var exception = Assert.ThrowsException<Exception>(() => flashcardState.GetQuiz(attempt));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Flashcard is already completed");

        }

        #endregion

        #region GetNextQuiz

        [TestMethod]
        public void InitFlashcardState_GetQuiz_ShouldReturnFirstQuiz()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizTypes(attempt, quizzes);

            var flashcardState = GetFlashcardState<InitFlashcardState>();

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            result.Should().BeEquivalentTo(firstQuiz);
        }

        [TestMethod]
        public void InitFlashcardState_GetQuiz_ShouldThrowIfNoFirstQuiz()
        {
            // Arrange
            var quizzes = GetQuizzes(10).Where(x => !x.IsFirst).ToList();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            AddQuizTypes(attempt, quizzes);

            var flashcardState = GetFlashcardState<InitFlashcardState>();

            // Act
            var exception = Assert.ThrowsException<NullReferenceException>(() => flashcardState.GetQuiz(attempt));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Default first quiz not found.");
        }

        [TestMethod]
        public void InProgressFlashcardState_GetQuiz_ShouldReturnFirstQuizIfNotCompleted()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var _ = flashcardState.GetNextState(data);

            var expectedQuiz = quizzes.First(x => x.IsFirst);

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            result.Should().BeEquivalentTo(expectedQuiz);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetQuiz_ShouldReturnDefaultQuizIfFirstCompleted()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var defaultQuizzes = quizzes.Where(x => !x.IsFirst && x.IsDefault);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.SeenQuizTypes.Add(new SeenQuizType(flashcardState.Id, firstQuiz.Id));
            flashcardState.CompletedQuizTypes.Add(new CompletedQuizType(flashcardState.Id, firstQuiz.Id));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var _ = flashcardState.GetNextState(data);

            var defaultQuizzeIds = defaultQuizzes.Select(x => x.Id).ToHashSet();

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            defaultQuizzeIds.Should().Contain(result.Id);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetQuiz_ShouldReturnNextQuizIfAllDefaultQuizesCompleted()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var defaultQuizzes = quizzes.Where(x => x.IsDefault);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.SeenQuizTypes = defaultQuizzes.Select(x => (new SeenQuizType(flashcardState.Id, x.Id))).ToList();
            flashcardState.CompletedQuizTypes = defaultQuizzes.Select(x => (new CompletedQuizType(flashcardState.Id, x.Id))).ToList();

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var _ = flashcardState.GetNextState(data);

            var defaultQuizzeIds = quizzes.Except(defaultQuizzes).Select(x => x.Id).ToHashSet();

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            defaultQuizzeIds.Should().Contain(result.Id);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetQuiz_ShouldReturnNotCompletedQuiz()
        {
            // Arrange
            var quizzes = GetQuizzes(10).ToList();
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);
            var take = quizzes.Count - 1;
            var completedQuizzes = quizzes.Take(take).ToList();

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.SeenQuizTypes = completedQuizzes.Select(x => (new SeenQuizType(flashcardState.Id, x.Id))).ToList();
            flashcardState.CompletedQuizTypes = completedQuizzes.Select(x => (new CompletedQuizType(flashcardState.Id, x.Id))).ToList();

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var _ = flashcardState.GetNextState(data);

            var expectedQuiz = quizzes.Except(completedQuizzes).First();

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            result.Should().Be(expectedQuiz);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetQuiz_ShouldReturnDifferentQuiz()
        {
            // Arrange
            var quizzes = GetQuizzes(3).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            var lastQuiz = quizzes.First(x => !x.IsFirst && x.Id != secondQuiz.Id);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);
            var take = quizzes.Count - 1;
            var completedQuizzes = quizzes.Take(take).ToList();

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizTypeId = secondQuiz.Id;
            flashcardState.CurrentQuizType = secondQuiz;
            flashcardState.SeenQuizTypes.Add(new SeenQuizType(flashcardState.Id, firstQuiz.Id));
            flashcardState.CompletedQuizTypes.Add(new CompletedQuizType(flashcardState.Id, firstQuiz.Id));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var _ = flashcardState.GetNextState(data);

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            result.Should().Be(lastQuiz);
        }

        [TestMethod]
        public void InProgressFlashcardState_GetQuiz_ShouldReturnTheSameQuizIfLast()
        {
            // Arrange
            var quizzes = GetQuizzes(2).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);
            var take = quizzes.Count - 1;
            var completedQuizzes = quizzes.Take(take).ToList();

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizTypeId = secondQuiz.Id;
            flashcardState.CurrentQuizType = secondQuiz;
            flashcardState.SeenQuizTypes.Add(new SeenQuizType(flashcardState.Id, firstQuiz.Id));
            flashcardState.CompletedQuizTypes.Add(new CompletedQuizType(flashcardState.Id, firstQuiz.Id));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = false
            };

            var _ = flashcardState.GetNextState(data);

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            result.Should().Be(secondQuiz);
        }


        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void InProgressFlashcardState_GetQuiz_ShouldNotReturnMultiselectIfFlashcardWithOnlyOneMeaning(int maxAnswersCount)
        {
            // Arrange
            var quizzes = GetQuizzes(3).ToList();
            var firstQuiz = quizzes.First(x => x.IsFirst);
            var secondQuiz = quizzes.First(x => !x.IsFirst);
            secondQuiz.MaxAnswersToSelect = maxAnswersCount;
            var lastQuiz = quizzes.First(x => !x.IsFirst && x.Id != secondQuiz.Id);
            var collection = Builder.Entities.GetCollection(Guid.NewGuid());
            var attempt = Builder.Entities.GetAttempt(collection);
            attempt.MinCompletedQuizzesPerCent = 100;
            AddQuizTypes(attempt, quizzes);

            var flashcardState = (InProgressFlashcardState)GetFlashcardState<InProgressFlashcardState>();
            flashcardState.CurrentQuizTypeId = firstQuiz.Id;
            flashcardState.CurrentQuizType = firstQuiz;
            flashcardState.Flashcard.FlashcardBase.Meanings.Add(Builder.Entities.GetMeaning(flashcardState.Flashcard.FlashcardBaseId));

            var data = new NextStateData()
            {
                UtcNow = DateTime.Now,
                Attempt = attempt,
                IsAnswerCorrect = true
            };

            var _ = flashcardState.GetNextState(data);

            // Act
            var result = flashcardState.GetQuiz(attempt);

            // Assert
            result.Should().Be(lastQuiz);
        }

        #endregion

        #region Auxiliary Methods




        private FlashcardState GetFlashcardState<T>() where T : FlashcardState
        {
            var flashcard = Builder.Entities.GetFlashcard(Guid.NewGuid(), Guid.NewGuid());
            flashcard.FlashcardBase = Builder.Entities.GetFlashcardBase();
            flashcard.FlashcardBaseId = flashcard.FlashcardBase.Id;

            return typeof(T).Name switch
            {
                nameof(InitFlashcardState) => Builder.Entities.GetInitFlashcardState(flashcard),
                nameof(InProgressFlashcardState) => Builder.Entities.GetInProfressFlashcardState(flashcard),
                nameof(CompletedFlashcardState) => Builder.Entities.GetCompletedFlashcardState(flashcard),
                _ => throw new InvalidOperationException($"Flascard state '{typeof(T).Name}' does not exist.")

            };
        }

        private IEnumerable<QuizType> GetQuizzes(int counter)
        {
            var quizzes = new List<QuizType>();

            for (var i = 0; i < counter;  i++)
            {
                var quiz = Builder.Entities.GetQuizType();
                quiz.IsFirst = i == 0;
                quiz.IsDefault = i % 4 == 0;
                quizzes.Add(quiz);
            }

            return quizzes;
        }

        private void AddQuizType(Attempt attempt, QuizType quizType)
        {
            if (attempt.AttemptQuizTypes is null)
                attempt.AttemptQuizTypes = new List<AttemptQuizType>();

            var attemptQuizType = new AttemptQuizType
            {
                AttemptId = attempt.Id,
                QuizTypeId = quizType.Id,
                QuizType = quizType
            };

            attempt.AttemptQuizTypes.Add(attemptQuizType);
        }

        private void AddQuizTypes(Attempt attempt, IEnumerable<QuizType> quizTypes)
        {
            foreach (var quizType in quizTypes)
            {
                AddQuizType(attempt, quizType);
            }
        }

        #endregion
    }
}
