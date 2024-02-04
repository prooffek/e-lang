using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using E_Lang.Domain.Enums;

namespace E_Lang.Domain.Entities
{
    public class Attempt : EntityBase
    {
        [Required]
        [MaxLength(1000)]
        public string Name { get; set; }

        [DefaultValue(10)]
        public int MaxFlashcardsPerStage { get; set; }
        
        [DefaultValue(1)]
        public int MaxQuizTypesPerFlashcard { get; set; }
        
        [DefaultValue(100)]
        public int MinCompletedQuizzesPerCent { get; set; }
        
        [DefaultValue(FlashcardOrder.AlphabeticalDesc)]
        public FlashcardOrder Order { get; set; }
        
        [DefaultValue(true)]
        public bool IncludeMeanings { get; set; }
        
        public ICollection<CustomProperty>? Properties { get; set; }
        
        public ICollection<QuizType>? QuizTypes { get; set; }

        [Required]
        public Guid CollectionId { get; set; }
        public Collection? Collection { get; set; }

        public AttemptStage? CurrentStage { get; set; }

        public ICollection<Flashcard>? CompletedFlashcards { get; set; }
    }
}
