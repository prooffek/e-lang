using E_Lang.Application.Attempts.Commands;
using E_Lang.Application.Common.DTOs;
using E_Lang.Application.Common.Interfaces.Repositories;
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
    }
}
