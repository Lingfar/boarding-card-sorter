using Application.BusCards.Dtos;
using Application.BusCards.Queries;
using Host.Dtos.Requests;
using Host.Dtos.Responses;
using Host.Mappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("bus-cards")]
public class BusCardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(BusCardDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BusCardDto[]>> GetAllAsync(CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new BusCardGetAll.Query(), cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(CreatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreatedDto>> CreateAsync(
        [FromBody] SyncBusCardDto request,
        CancellationToken cancellationToken = default)
        => Ok(new CreatedDto(await mediator.Send(request.MapToBusCardCreateCommand(), cancellationToken)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BusCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BusCardDto>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new BusCardGetById.Query(id), cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] SyncBusCardDto request,
        CancellationToken cancellationToken = default)
    {
        var command = request.MapToBusCardUpdateCommand();
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
