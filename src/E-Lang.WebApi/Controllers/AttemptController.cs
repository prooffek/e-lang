using E_Lang.Application.Attempts.Commands;
using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Application.Exercises.Requests;
using E_Lang.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Lang.WebApi.Controllers
{
    public class AttemptController : BaseController<Attempt, AttemptDto>
    {
        public AttemptController(IMediator mediator, IAttemptRepository repository) : base(mediator, repository)
        {
        }

        [HttpPost]
        public async Task<ActionResult<AttemptDto>> AddAttempt([FromBody] AddAttemptDto attempt)
        {
            return await _mediator.Send(new AddAttemptRequest { Attempt = attempt });
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAttempt([FromBody] Guid attemptId)
        {
            await _mediator.Send(new DeleteAttemptRequest() {AttemptId = attemptId});
            return Ok();
        }

        [HttpPost("get-exercise")]
        public async Task<ActionResult<NextExerciseDto>> GetExercise([FromBody] Guid attemptId, Guid? flashcardStateId,
            bool? isAnswerCorrect)
        {
            GetNextExerciseRequest request = new()
            {
                AttemptId = attemptId,
                FlashcardStateId = flashcardStateId,
                IsAnswerCorrect = isAnswerCorrect
            };

            return Ok(await _mediator.Send(request));
        }
    }
}
