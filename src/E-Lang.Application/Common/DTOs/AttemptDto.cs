using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;
using E_Lang.Domain.Enums;
using Mapster;

namespace E_Lang.Application.Common.DTOs
{
    public class AttemptDto : IMapper<Attempt>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CollectionId { get; set; }
        public string CollectionName { get; set; }
        public AttemptStageDto CurrentStage { get; set; }
        public int MaxFlashcardsPerStage { get; set; }
        public int MaxQuizTypesPerFlashcard { get; set; }
        public int MinCompletedQuizzesPerCent { get; set; }
        public FlashcardOrder Order { get; set; }
        public bool IncludeMeanings { get; set; }
        public ICollection<CustomPropertyDto>? Properties { get; set; }
        public ICollection<QuizTypeDto>? QuizTypes { get; set; }
        public int CompletedFlashcardsCount { get; set; }
        public int AllFlashcardsCount { get; set; }
        public DateTime? LastSeenOn { get; set; }

        public void Map(TypeAdapterConfig config)
        {
            config.NewConfig<Attempt, AttemptDto>()
                .Map(dest => dest.CollectionName, src => src.Collection.Name)
                .Map(dest => dest.MaxFlashcardsPerStage, src => src.MaxFlashcardsPerStage <= 0 ? 10 : src.MaxFlashcardsPerStage)
                .Map(dest => dest.MaxQuizTypesPerFlashcard, src => src.MaxQuizTypesPerFlashcard <= 0 ? 1 : src.MaxQuizTypesPerFlashcard)
                .Map(dest => dest.MinCompletedQuizzesPerCent, src => src.MinCompletedQuizzesPerCent <= 0 ? 100 : src.MinCompletedQuizzesPerCent)
                .Map(dest => dest.CompletedFlashcardsCount, src => src.CompletedFlashcards != null ? src.CompletedFlashcards.Count : 0)
                .Map(dest => dest.AllFlashcardsCount, src => src.Collection != null && src.Collection.Flashcards != null ? src.Collection.Flashcards.Count : 0)
                .Map(dest => dest.LastSeenOn, src => src.ModifiedOn);
        }
    }
}
