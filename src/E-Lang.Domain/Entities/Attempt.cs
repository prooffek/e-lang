using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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


        [Required]
        public Guid CollectionId { get; set; }
        public Collection? Collection { get; set; }


        public ICollection<CustomProperty>? Properties { get; set; }

        public ICollection<AttemptQuizType> AttemptQuizTypes { get; set; } = new List<AttemptQuizType>();

        public ICollection<AttemptStage>? AttemptStages { get; set; }


        [NotMapped]
        public ICollection<QuizType> QuizTypes => AttemptQuizTypes
              ?.Where(x => x.QuizType != null)
              .Select(x => x.QuizType!)
              .ToList()
          ?? new List<QuizType>();

        [NotMapped]
        public AttemptStage? CurrentStage => AttemptStages?.SingleOrDefault(x => x.Stage != AttemptStageType.Complete);

        [NotMapped]
        public ICollection<Flashcard>? CompletedFlashcards => AttemptStages
            ?.Where(x => x.Stage == AttemptStageType.Complete && x.Flashcards != null)
            .SelectMany(x => x.Flashcards!.Where(x => x.FlashcardId != null).Select(y => y.Flashcard))
            .ToList();
    }
}
