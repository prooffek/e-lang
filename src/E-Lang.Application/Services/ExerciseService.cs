using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Interfaces;
using E_Lang.Application.Models;
using E_Lang.Domain.Entities;
using Mapster;
using MapsterMapper;

namespace E_Lang.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private const int ALL_ANSWERS_NUMBER = 4;

        private readonly IMapper _mapper;
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFlashcardStateRepository _flashcardStateRepository;

        public ExerciseService(IMapper mapper, IFlashcardRepository flashcardRepository, IDateTimeProvider dateTimeProvider,
            IFlashcardStateRepository flashcardStateRepository)
        {
            _mapper = mapper;
            _flashcardRepository = flashcardRepository;
            _dateTimeProvider = dateTimeProvider;
            _flashcardStateRepository = flashcardStateRepository;
        }

        public async Task<ExerciseDto> GetExercise(ExerciseData data, CancellationToken cancellationToken)
        {
            if (data.QuizType.MaxAnswersToSelect < 1 || data.QuizType.MaxAnswersToSelect >= ALL_ANSWERS_NUMBER)
                throw new ArgumentException($"The maximal number of answers to select should range between 1 and 3. The current value is: {data.QuizType.MaxAnswersToSelect}.");

            var exercise = _mapper.Map<ExerciseDto>(data.QuizType)
                ?? throw new ArgumentNullException(nameof(QuizType), $"{nameof(QuizType)} object not mapped successfully to {nameof(ExerciseDto)}");

            exercise.AttemptId = data.AttemptId;
            exercise.FlashcardStateId = data.FlashcardStateId;
            exercise.WordOrPhrase = data.FlashcardBase.WordOrPhrase;

            if (data.QuizType.IsSelect)
            {
                await SetAnswers(exercise, data.CollectionId, data.FlashcardBase.Meanings, data.QuizType, cancellationToken);
            }
            
            return exercise;
        }

        public async Task<ExerciseDto?> GetNextExercise(Attempt attempt, Guid? flashcardStateId, CancellationToken cancellationToken)
        {
            var nextFlashcard = await GetRandomFlashcardState(attempt.CurrentStage.Flashcards, flashcardStateId, cancellationToken);

            if (nextFlashcard is null)
                return null;

            var nextQuiz = nextFlashcard.GetQuiz(attempt);

            ExerciseData exerciseData = new(attempt.Id, nextFlashcard.Id, attempt.CollectionId, nextFlashcard.Flashcard.FlashcardBase, nextQuiz);

            return await GetExercise(exerciseData, cancellationToken);
        }

        private async Task SetAnswers(ExerciseDto exercise, Guid collectionId, IEnumerable<Meaning> meanings, QuizType quizType, CancellationToken cancellationToken)
        {
            var meaningsFromFlashcard = GetAnswers(meanings, quizType.MaxAnswersToSelect);
            var meaningsFromDb = await _flashcardRepository.GetRadomAnswers(collectionId, meanings, ALL_ANSWERS_NUMBER - meaningsFromFlashcard.Count(), cancellationToken);

            exercise.CorrectAnswers = quizType.IsSelectCorrect ? meaningsFromFlashcard : meaningsFromDb;
            exercise.IncorrectAnswers = quizType.IsSelectCorrect ? meaningsFromDb : meaningsFromFlashcard;
        }

        private IEnumerable<AnswerDto> GetAnswers(IEnumerable<Meaning> meanings, int answersNumber)
        {
            return meanings
                .OrderBy(x => Guid.NewGuid())
                .Take(answersNumber)
                .Adapt<IEnumerable<AnswerDto>>()
                 ?? throw new ArgumentOutOfRangeException();
        }

        private async Task<FlashcardState?> GetRandomFlashcardState(IEnumerable<FlashcardState> flashcards, Guid? flashcardStateId, CancellationToken cancellationToken)
        {
            var flashcardStates = flashcards.Where(f => !(f is CompletedFlashcardState));

            if (!flashcardStates.Any())
                return null;

            FlashcardState? flashcardState;

            if (flashcardStates.Count() == 1)
            {
                flashcardState = flashcardStates.First();
            }
            else
            {
                flashcardState = flashcards
                                     .FirstOrDefault(x => x is InProgressFlashcardState f && f.ShowAgainOn.HasValue && f.ShowAgainOn <= _dateTimeProvider.UtcNow)

                                 ?? flashcards
                                     .Where(f => !(f is CompletedFlashcardState) && (!flashcardStateId.HasValue || f.Id != flashcardStateId))
                                     .MinBy(x => Guid.NewGuid());
            }

            if (flashcardState is null)
                throw new ArgumentOutOfRangeException();

            return await _flashcardStateRepository.GetByIdAsync(flashcardState.Id, cancellationToken)
                   ?? throw new NullReferenceException($"Flashcard state with Id {flashcardState.Id} not found.");
        }
    }
}
