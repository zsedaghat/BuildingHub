using BuildingHub.Modules.Community.Application.Buildings.Create;
using BuildingHub.Modules.Community.Application.Buildings.GetList;
using BuildingHub.Modules.Community.Application.Buildings.GetById;
using BuildingHub.Modules.Community.Application.Buildings.Update;
using BuildingHub.Modules.Community.Application.Buildings.Delete;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/buildings")]
public class BuildingsController : ControllerBase
{
    private readonly CreateBuildingCommandHandler _createHandler;
    private readonly GetBuildingsQueryHandler _getBuildingsHandler;
    private readonly GetBuildingsByCommunityIdQueryHandler _getByCommunityHandler;
    private readonly GetBuildingByIdQueryHandler _getByIdHandler;
    private readonly UpdateBuildingCommandHandler _updateHandler;
    private readonly DeleteBuildingCommandHandler _deleteHandler;

    public BuildingsController(
        CreateBuildingCommandHandler createHandler,
        GetBuildingsQueryHandler getBuildingsHandler,
        GetBuildingsByCommunityIdQueryHandler getByCommunityHandler,
        GetBuildingByIdQueryHandler getByIdHandler,
        UpdateBuildingCommandHandler updateHandler,
        DeleteBuildingCommandHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getBuildingsHandler = getBuildingsHandler;
        _getByCommunityHandler = getByCommunityHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBuildingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.Handle(
            command,
            cancellationToken);

        return Ok(result);
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _getBuildingsHandler.Handle(
            new GetBuildingsQuery(),
            cancellationToken);

        return Ok(result);
    }

    // GET BY COMMUNITY
    [HttpGet("community/{communityId:guid}")]
    public async Task<IActionResult> GetByCommunity(
        Guid communityId,
        CancellationToken cancellationToken)
    {
        var result = await _getByCommunityHandler.Handle(
            new GetBuildingsByCommunityIdQuery(communityId),
            cancellationToken);

        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.Handle(
            new GetBuildingByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateBuildingCommand command,
        CancellationToken cancellationToken)
    {
        var commandWithId = command with
        {
            Id = id
        };

        var result = await _updateHandler.Handle(
            commandWithId,
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // DELETE
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.Handle(
            new DeleteBuildingCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}