using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using Mapster;

namespace E_Lang.Application.Common.DTOs;

public class ExerciseDto : IMapper<QuizType>
{
    public Guid AttemptId { get; set; }

    public Guid FlashcardStateId { get; set; }
    
    public string Instruction { get; set; }
    
    public string WordOrPhrase { get; set; }
    
    public IEnumerable<AnswerDto> CorrectAnswers { get; set; }
    
    public IEnumerable<AnswerDto> IncorrectAnswers { get; set; }

    public bool IsSelect { get; set; }

    public bool IsMultiSelect { get; set; }

    public bool IsSelectMissing { get; set; }

    public bool IsMatch { get; set; }

    public bool IsArrange { get; set; }

    public bool IsInput { get; set; }

    public bool IsFillInBlank { get; set; }

    public void Map(TypeAdapterConfig config)
    {
        config.NewConfig<QuizType, ExerciseDto>()
            .Map(dest => dest.IsMultiSelect, src => src.MaxAnswersToSelect > 1);
    }
}