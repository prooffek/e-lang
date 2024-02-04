﻿using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class AttemptQuizType : EntityBase
    {
        [Required]
        public Guid QuizTypeId { get; set; }
        
        [Required]
        public Guid AttemptId { get; set; }
    }
}
