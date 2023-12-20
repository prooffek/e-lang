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
        
        public bool IsMultiselect { get; set; }
        
        public bool IsSelectCorrect { get; set; }
        
        public bool IsSelectMissing { get; set; }
        
        public bool IsMatch { get; set; }
        
        public bool IsArrange { get; set; }
        
        public bool IsInput { get; set; }
        
        public bool IsFillInBlank { get; set; }
    }
}
