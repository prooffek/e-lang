using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Models;

namespace E_Lang.Application.Interfaces
{
    public interface IExerciseService
    {
        Task<ExerciseDto> GetExercise(ExerciseData data, CancellationToken cancellationToken);
    }
}
