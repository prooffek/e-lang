using E_Lang.Application.Common.Interfaces;
using E_Lang.Domain.Entities;

namespace E_Lang.Application.Common.DTOs;

public class FlashcardDto : IMapper<Flashcard>
{
    public Guid Id { get; set; }
}