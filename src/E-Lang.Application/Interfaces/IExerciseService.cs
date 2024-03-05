using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Models;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Interfaces
{
    public interface IExerciseService
    {
        Task<ExerciseDto> GetExercise(ExerciseData data, CancellationToken cancellationToken);

        Task<ExerciseDto?> GetNextExercise(Attempt attempt, Guid? flashcardStateId,
            CancellationToken cancellationToken);
    }
}
