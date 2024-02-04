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

        public ExerciseService(IMapper mapper, IFlashcardRepository flashcardRepository)
        {
            _mapper = mapper;
            _flashcardRepository = flashcardRepository;
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
            await SetAnswers(exercise, data.CollectionId, data.FlashcardBase.Meanings, data.QuizType, cancellationToken);

            return exercise;
        }

        private async Task SetAnswers(ExerciseDto exercise, Guid collectionId, IEnumerable<Meaning> meanings, QuizType quizType, CancellationToken cancellationToken)
        {
            var meaningsFromFlashcard = GetAnswers(meanings, quizType.MaxAnswersToSelect);
            var meaningsFromDb = await _flashcardRepository.GetRadomAnswers(collectionId, meanings, ALL_ANSWERS_NUMBER - quizType.MaxAnswersToSelect, cancellationToken);

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
    }
}
