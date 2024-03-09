using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.DTOs
{
    public class QuizTypeDto : IMapper<QuizType>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
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
