using Application.TrainCards.Dtos;
using Application.TrainCards.Queries;
using Host.Dtos.Requests;
using Host.Dtos.Responses;
using Host.Mappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("train-cards")]
public class TrainCardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(TrainCardDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TrainCardDto[]>> GetAllAsync(CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new TrainCardGetAll.Query(), cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(CreatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreatedDto>> CreateAsync(
        [FromBody] SyncTrainCardDto request,
        CancellationToken cancellationToken = default)
        => Ok(new CreatedDto(await mediator.Send(request.MapToTrainCardCreateCommand(), cancellationToken)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TrainCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TrainCardDto>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new TrainCardGetById.Query(id), cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] SyncTrainCardDto request,
        CancellationToken cancellationToken = default)
    {
        var command = request.MapToTrainCardUpdateCommand();
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
