using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities
{
    public class CustomProperty : EntityBase
    {
        [Required]
        [MaxLength(1000)]
        public string Name { get; set; }
    }
}
