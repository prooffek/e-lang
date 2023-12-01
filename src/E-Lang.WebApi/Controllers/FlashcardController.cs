using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Domain.Entities;
using MediatR;

namespace E_Lang.WebApi.Controllers
{
    public class FlashcardController : BaseController<Flashcard, FlashcardDto>
    {
        public FlashcardController(IMediator mediator, IFlashcardRepository repository) : base(mediator, repository)
        {
        }
    }
}
