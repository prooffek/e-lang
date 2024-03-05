using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Application.Models;
using E_Lang.Application.Services;
using E_Lang.Builder;
using E_Lang.Domain.Entities;
using FluentAssertions;
using MapsterMapper;
using Moq;

namespace E_Lang.Application.UnitTests.Services
{
    [TestClass]
    public class ExerciseServiceTests : Setup
    {
        private static Mock<IMapper> _mapper;
        private static Mock<IFlashcardRepository> _flashcardRepository;
        private static Mock<IDateTimeProvider> _dateTimeProvider;
        private static Mock<IFlashcardStateRepository> _flashcardStateRepository;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            InitClass();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            InitTest();
            _mapper = new Mock<IMapper>();
            _flashcardRepository = new Mock<IFlashcardRepository>();
            _dateTimeProvider = new Mock<IDateTimeProvider>();
            _flashcardStateRepository = new Mock<IFlashcardStateRepository>();
        }

        [TestCleanup]
        public void CleanUp()
        {
            CleanupTest();
        }

        [TestMethod]
        public async Task ExerciseService_GetExercise_ShouldThrowIfMappedToNull()
        {
            // Arrange
            _mapper.Setup(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()))
                .Returns((ExerciseDto)null)
                .Verifiable();

            var quizType = Entities.GetQuizType();
            var service = new ExerciseService(_mapper.Object, _flashcardRepository.Object, _dateTimeProvider.Object, _flashcardStateRepository.Object);

            ExerciseData exerciseData = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new FlashcardBase(), quizType);

            // Act
            var result = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => 
                await service.GetExercise(exerciseData, default));

            // Assert
            result.Should().NotBeNull();
            result.ParamName.Should().Be(nameof(QuizType));
            result.Message.Should().Be($"{nameof(QuizType)} object not mapped successfully to {nameof(ExerciseDto)} (Parameter 'QuizType')");

            _mapper.Verify(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()), Times.Once);
            _flashcardRepository.Verify(r => 
                r.GetRadomAnswers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Meaning>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(20)]
        public async Task ExerciseService_GetExercise_ShoulThrowIfWrongMaxAnswersToSelect(int answersCount)
        {
            // Arrange
            var flashcardStateId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var quizType = Entities.GetQuizType();
            quizType.MaxAnswersToSelect = answersCount;
            var exerciseDto = GetExerciseDto();
            var flashcardBase = GetFlashcardBase(answersCount);
            var correctAnswers = GetAnswers(flashcardBase.Meanings);
            var incorrectAnswers = GetAnswers(GetMeanings(Entities.GetFlashcardBase(), 4 - answersCount));

            _mapper.Setup(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()))
                .Returns(exerciseDto)
                .Verifiable();

            _flashcardRepository.Setup(r =>
                r.GetRadomAnswers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Meaning>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incorrectAnswers))
                .Verifiable();

            var service = new ExerciseService(_mapper.Object, _flashcardRepository.Object, _dateTimeProvider.Object, _flashcardStateRepository.Object);
            ExerciseData exerciseData = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new FlashcardBase(), quizType);

            // Act
            var result = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await service.GetExercise(exerciseData, default));

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be($"The maximal number of answers to select should range between 1 and 3. The current value is: {quizType.MaxAnswersToSelect}.");
        }

        [TestMethod]
        public async Task ExerciseService_GetExercise_ShoulFillAllProperties()
        {
            // Arrange
            var flashcardStateId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var quizType = Entities.GetQuizType();
            var exerciseDto = GetExerciseDto();
            var flashcardBase = GetFlashcardBase(1);
            var answers = GetAnswers(flashcardBase.Meanings);

            _mapper.Setup(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()))
                .Returns(exerciseDto)
                .Verifiable();

            _flashcardRepository.Setup(r => 
                r.GetRadomAnswers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Meaning>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(answers))
                .Verifiable();

            var service = new ExerciseService(_mapper.Object, _flashcardRepository.Object, _dateTimeProvider.Object, _flashcardStateRepository.Object);
            ExerciseData exerciseData = new(Guid.NewGuid(), flashcardStateId, collectionId, flashcardBase, quizType);

            // Act
            var result = await service.GetExercise(exerciseData, default);

            // Assert
            result.Should().NotBeNull();
            result.AttemptId.Should().Be(exerciseData.AttemptId);
            result.FlashcardStateId.Should().Be(flashcardStateId);
            result.Instruction.Should().Be(quizType.Instruction);
            result.WordOrPhrase.Should().Be(flashcardBase.WordOrPhrase);
            result.CorrectAnswers.Should().NotBeNull();
            result.IncorrectAnswers.Should().NotBeNull();
            result.IsSelect.Should().Be(quizType.IsSelect);
            result.IsMultiSelect.Should().Be(quizType.MaxAnswersToSelect > 1);
            result.IsSelectMissing.Should().Be(quizType.IsSelectMissing);
            result.IsMatch.Should().Be(quizType.IsMatch);
            result.IsArrange.Should().Be(quizType.IsArrange);
            result.IsInput.Should().Be(quizType.IsInput);
            result.IsFillInBlank.Should().Be(quizType.IsFillInBlank);
        }

        [TestMethod]
        public async Task ExerciseService_GetExercise_ShoulAssignCorrectAnswersFromFlashcardBase()
        {
            // Arrange
            var flashcardStateId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var quizType = Entities.GetQuizType();
            quizType.IsSelectCorrect = true;
            var exerciseDto = GetExerciseDto();
            var flashcardBase = GetFlashcardBase(1);
            var correctAnswers = GetAnswers(flashcardBase.Meanings);
            var incorrectAnswers = GetAnswers(GetMeanings(Entities.GetFlashcardBase(), 3));

            _mapper.Setup(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()))
                .Returns(exerciseDto)
                .Verifiable();

            _flashcardRepository.Setup(r =>
                r.GetRadomAnswers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Meaning>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incorrectAnswers))
                .Verifiable();

            var service = new ExerciseService(_mapper.Object, _flashcardRepository.Object, _dateTimeProvider.Object, _flashcardStateRepository.Object);
            ExerciseData exerciseData = new(Guid.NewGuid(), flashcardStateId, collectionId, flashcardBase, quizType);

            // Act
            var result = await service.GetExercise(exerciseData, default);

            // Assert
            result.CorrectAnswers.Should().NotBeNull()
                .And.HaveCount(1)
                .And.BeEquivalentTo(correctAnswers);
            result.IncorrectAnswers.Should().NotBeNull()
                .And.HaveCount(3)
                .And.BeEquivalentTo(incorrectAnswers);
        }

        [TestMethod]
        public async Task ExerciseService_GetExercise_ShoulAssignCorrectAnswersFromDatabase()
        {
            // Arrange
            var flashcardStateId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var quizType = Entities.GetQuizType();
            quizType.IsSelectCorrect = false;
            var exerciseDto = GetExerciseDto();
            var flashcardBase = GetFlashcardBase(1);
            var answers = GetAnswers(flashcardBase.Meanings);
            var answersFromDb = GetAnswers(GetMeanings(Entities.GetFlashcardBase(), 3));

            _mapper.Setup(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()))
                .Returns(exerciseDto)
                .Verifiable();

            _flashcardRepository.Setup(r =>
                r.GetRadomAnswers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Meaning>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(answersFromDb))
                .Verifiable();

            var service = new ExerciseService(_mapper.Object, _flashcardRepository.Object, _dateTimeProvider.Object, _flashcardStateRepository.Object);
            ExerciseData exerciseData = new(Guid.NewGuid(), flashcardStateId, collectionId, flashcardBase, quizType);

            // Act
            var result = await service.GetExercise(exerciseData, default);

            // Assert
            result.CorrectAnswers.Should().NotBeNull()
                .And.HaveCount(3)
                .And.BeEquivalentTo(answersFromDb);
            result.IncorrectAnswers.Should().NotBeNull()
                .And.HaveCount(1)
                .And.BeEquivalentTo(answers);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task ExerciseService_GetExercise_ShoulReturnCorrectNumberOfAnswers(int answersCount)
        {
            // Arrange
            var flashcardStateId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var quizType = Entities.GetQuizType();
            quizType.MaxAnswersToSelect = answersCount;
            var exerciseDto = GetExerciseDto();
            var flashcardBase = GetFlashcardBase(answersCount);
            var correctAnswers = GetAnswers(flashcardBase.Meanings);
            var incorrectAnswers = GetAnswers(GetMeanings(Entities.GetFlashcardBase(), 4 - answersCount));

            _mapper.Setup(m => m.Map<ExerciseDto>(It.IsAny<QuizType>()))
                .Returns(exerciseDto)
                .Verifiable();

            _flashcardRepository.Setup(r =>
                r.GetRadomAnswers(It.IsAny<Guid>(), It.IsAny<IEnumerable<Meaning>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incorrectAnswers))
                .Verifiable();

            var service = new ExerciseService(_mapper.Object, _flashcardRepository.Object, _dateTimeProvider.Object, _flashcardStateRepository.Object);
            ExerciseData exerciseData = new(Guid.NewGuid(), flashcardStateId, collectionId, flashcardBase, quizType);

            // Act
            var result = await service.GetExercise(exerciseData, default);

            // Assert
            result.CorrectAnswers.Should().HaveCount(answersCount);

            var answers = result.CorrectAnswers.Concat(incorrectAnswers);
            answers.Should().HaveCount(4);
        }

        private ExerciseDto GetExerciseDto()
        {
            return new ExerciseDto
            {
                Instruction = "Chose the right answer",
                IsSelect = true,
                IsSelectMissing = false,
                IsMatch = false,
                IsArrange = false,
                IsInput = false,
                IsFillInBlank = false,
            };
        }

        private FlashcardBase GetFlashcardBase(int meaningsCounter)
        {
            var flashcardBase = Entities.GetFlashcardBase();
            flashcardBase.Meanings = GetMeanings(flashcardBase, meaningsCounter);
            return flashcardBase;
        }

        private List<Meaning> GetMeanings(FlashcardBase flashcardBase, int counter)
        {
            List<Meaning> meanings = new List<Meaning>();

            for (int i = 0; i < counter; i++)
            {
                var meaning = Entities.GetMeaning(flashcardBase.Id);
                meaning.Value = $"{meaning.Value} {i}";
                meaning.FlashcardBase = flashcardBase;

                meanings.Add(meaning);
            }

            return meanings;
        }

        private IEnumerable<AnswerDto> GetAnswers(IEnumerable<Meaning> meanings)
        {
            return meanings.Select(m => new AnswerDto { Value = m.Value });
        }
    }
}
