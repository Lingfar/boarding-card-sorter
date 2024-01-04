using Application.PlaneCards.Dtos;
using Application.PlaneCards.Queries;
using Host.Dtos.Requests;
using Host.Dtos.Responses;
using Host.Mappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("plane-cards")]
public class PlaneCardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PlaneCardDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PlaneCardDto[]>> GetAllAsync(CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new PlaneCardGetAll.Query(), cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(CreatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreatedDto>> CreateAsync(
        [FromBody] SyncPlaneCardDto request,
        CancellationToken cancellationToken = default)
        => Ok(new CreatedDto(await mediator.Send(request.MapToPlaneCardCreateCommand(), cancellationToken)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlaneCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PlaneCardDto>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.Send(new PlaneCardGetById.Query(id), cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] SyncPlaneCardDto request,
        CancellationToken cancellationToken = default)
    {
        var command = request.MapToPlaneCardUpdateCommand();
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
