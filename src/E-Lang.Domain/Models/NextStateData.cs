using E_Lang.Domain.Entities;

namespace E_Lang.Domain.Models;

public struct NextStateData
{
    public DateTime UtcNow { get; set; }
    public bool? IsAnswerCorrect { get; set; }
    public Attempt? Attempt { get; set; }
}