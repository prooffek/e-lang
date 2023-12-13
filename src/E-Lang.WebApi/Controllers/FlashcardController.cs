using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Flashcards.Commands;
using E_Lang.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Lang.WebApi.Controllers
{
    public class FlashcardController : BaseController<Flashcard, FlashcardDto>
    {
        public FlashcardController(IMediator mediator, IFlashcardRepository repository) : base(mediator, repository)
        {
        }

        [HttpPut("add-or-update")]
        public async Task<ActionResult<FlashcardDto>> AddOrUpdateFlashcard([FromBody] AddOrUpdateFlashcardDto flashcard)
        {
            return await _mediator.Send(new AddOrUpdateFlashcardRequest { Flashcard = flashcard });
        }

        [HttpDelete("remove-flashcard")]
        public async Task<IActionResult> DeleteFlashcard([FromBody] Guid? flashcardId)
        {
            await _mediator.Send(new RemoveFlashcardRequest { FlashcardId = flashcardId });
            return Ok();
        }

        [HttpDelete("remove-flashcards")]
        public async Task<IActionResult> DeleteFlashcards([FromBody] IEnumerable<Guid>? flashcardIds)
        {
            await _mediator.Send(new RemoveFlashcardsRequest { FlashcardIds = flashcardIds });
            return Ok();
        }
    }
}
