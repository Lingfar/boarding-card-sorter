using Application.BoardingCards.Commands;
using Application.BoardingCards.Dtos;
using Application.BoardingCards.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("boarding-cards")]
public class BoardingCardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BoardingCardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BoardingCardDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new BoardingCardGetAll.Query(), cancellationToken));

    [HttpPost("order")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> OrderAndGetJourneyAsync(CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new BoardingCardOrder.Command(), cancellationToken));
}
