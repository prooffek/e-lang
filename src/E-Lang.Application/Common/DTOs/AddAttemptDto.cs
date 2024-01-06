using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;

namespace E_Lang.Application.Common.DTOs
{
    public class AddAttemptDto : IMapper<Attempt>
    {
        public string Name { get; set; }
        public Guid CollectionId { get; set; }
        public int MaxFlashcardsPerStage { get; set; }
        public int MaxQuizTypesPerFlashcard { get; set; }
        public int MinCompletedQuizzesPerCent { get; set; }
        public FlashcardOrder Order { get; set; }
        public bool IncludeMeanings { get; set; }
        public ICollection<CustomPropertyDto>? Properties { get; set; }
        public ICollection<QuizTypeDto>? QuizTypes { get; set; }
    }
}
