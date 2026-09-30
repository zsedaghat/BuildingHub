using BuildingHub.Modules.Community.Application.Floors.Create;
using BuildingHub.Modules.Community.Application.Floors.Delete;
using BuildingHub.Modules.Community.Application.Floors.GetByBuildingId;
using BuildingHub.Modules.Community.Application.Floors.GetById;
using BuildingHub.Modules.Community.Application.Floors.GetList;
using BuildingHub.Modules.Community.Application.Floors.Update;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/floors")]
public sealed class FloorsController : ControllerBase
{
    private readonly CreateFloorCommandHandler _createHandler;
    private readonly GetFloorsQueryHandler _getListHandler;
    private readonly GetFloorsByBuildingIdQueryHandler _getByBuildingHandler;
    private readonly GetFloorByIdQueryHandler _getByIdHandler;
    private readonly UpdateFloorCommandHandler _updateHandler;
    private readonly DeleteFloorCommandHandler _deleteHandler;

    public FloorsController(
        CreateFloorCommandHandler createHandler,
        GetFloorsQueryHandler getListHandler,
        GetFloorsByBuildingIdQueryHandler getByBuildingHandler,
        GetFloorByIdQueryHandler getByIdHandler,
        UpdateFloorCommandHandler updateHandler,
        DeleteFloorCommandHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getListHandler = getListHandler;
        _getByBuildingHandler = getByBuildingHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFloorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.Handle(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _getListHandler.Handle(
            new GetFloorsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("building/{buildingId:guid}")]
    public async Task<IActionResult> GetByBuilding(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var result = await _getByBuildingHandler.Handle(
            new GetFloorsByBuildingIdQuery(buildingId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.Handle(
            new GetFloorByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateFloorCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and command id must match.");

        var result = await _updateHandler.Handle(
            command,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.Handle(
            new DeleteFloorCommand(id),
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}