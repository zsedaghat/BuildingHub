using BuildingHub.Modules.Community.Application.Units.Create;
using BuildingHub.Modules.Community.Application.Units.Delete;
using BuildingHub.Modules.Community.Application.Units.GetByBuildingId;
using BuildingHub.Modules.Community.Application.Units.GetByFloorId;
using BuildingHub.Modules.Community.Application.Units.GetById;
using BuildingHub.Modules.Community.Application.Units.GetList;
using BuildingHub.Modules.Community.Application.Units.Update;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/units")]
public sealed class UnitsController : ControllerBase
{
    private readonly CreateUnitCommandHandler _createHandler;
    private readonly GetUnitsQueryHandler _getListHandler;
    private readonly GetUnitsByBuildingIdQueryHandler _getByBuildingHandler;
    private readonly GetUnitsByFloorIdQueryHandler _getByFloorHandler;
    private readonly GetUnitByIdQueryHandler _getByIdHandler;
    private readonly UpdateUnitCommandHandler _updateHandler;
    private readonly DeleteUnitCommandHandler _deleteHandler;

    public UnitsController(
        CreateUnitCommandHandler createHandler,
        GetUnitsQueryHandler getListHandler,
        GetUnitsByBuildingIdQueryHandler getByBuildingHandler,
        GetUnitsByFloorIdQueryHandler getByFloorHandler,
        GetUnitByIdQueryHandler getByIdHandler,
        UpdateUnitCommandHandler updateHandler,
        DeleteUnitCommandHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getListHandler = getListHandler;
        _getByBuildingHandler = getByBuildingHandler;
        _getByFloorHandler = getByFloorHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUnitCommand command,
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
            new GetUnitsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("building/{buildingId:guid}")]
    public async Task<IActionResult> GetByBuilding(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var result = await _getByBuildingHandler.Handle(
            new GetUnitsByBuildingIdQuery(buildingId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("floor/{floorId:guid}")]
    public async Task<IActionResult> GetByFloor(
        Guid floorId,
        CancellationToken cancellationToken)
    {
        var result = await _getByFloorHandler.Handle(
            new GetUnitsByFloorIdQuery(floorId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.Handle(
            new GetUnitByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUnitCommand command,
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
            new DeleteUnitCommand(id),
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}