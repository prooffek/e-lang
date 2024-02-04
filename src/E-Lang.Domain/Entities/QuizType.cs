using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class QuizType : EntityBase
    {
        [Required]
        [MaxLength(1000)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(10000)]
        public string Instruction { get; set; }
        
        public bool IsSelect { get; set; }

        [DefaultValue(1)]
        [Range(1, 3)]
        public int MaxAnswersToSelect { get; set; }

        public bool IsSelectCorrect { get; set; }
        
        public bool IsSelectMissing { get; set; }
        
        public bool IsMatch { get; set; }
        
        public bool IsArrange { get; set; }
        
        public bool IsInput { get; set; }
        
        public bool IsFillInBlank { get; set; }

        /// <summary>
        /// If true, the quiz is required for any flashcard.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        [DefaultValue(false)]
        public bool IsFirst { get; set; }

        public Guid OwnerId { get; set; }
    }
}
