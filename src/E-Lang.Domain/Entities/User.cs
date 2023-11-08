using System.ComponentModel.DataAnnotations;

namespace E_Lang.Domain.Entities;

public class User : EntityBase
{
    [Required]
    public string UserName { get; set; }
    
    [EmailAddress]
    [Required]
    public string Email { get; set; }
}