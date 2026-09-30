using BuildingHub.Modules.Community.Application.UnitPersons.Create;
using BuildingHub.Modules.Community.Application.UnitPersons.Delete;
using BuildingHub.Modules.Community.Application.UnitPersons.GetById;
using BuildingHub.Modules.Community.Application.UnitPersons.GetByPersonId;
using BuildingHub.Modules.Community.Application.UnitPersons.GetByUnitId;
using BuildingHub.Modules.Community.Application.UnitPersons.GetList;
using BuildingHub.Modules.Community.Application.UnitPersons.Update;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/unit-persons")]
public class UnitPersonsController : ControllerBase
{
    private readonly CreateUnitPersonCommandHandler _createHandler;
    private readonly GetUnitPersonsQueryHandler _getListHandler;
    private readonly GetUnitPersonByIdQueryHandler _getByIdHandler;
    private readonly GetUnitPersonsByUnitIdQueryHandler _getByUnitIdHandler;
    private readonly GetUnitPersonsByPersonIdQueryHandler _getByPersonIdHandler;
    private readonly UpdateUnitPersonCommandHandler _updateHandler;
    private readonly DeleteUnitPersonCommandHandler _deleteHandler;

    public UnitPersonsController(
        CreateUnitPersonCommandHandler createHandler,
        GetUnitPersonsQueryHandler getListHandler,
        GetUnitPersonByIdQueryHandler getByIdHandler,
        GetUnitPersonsByUnitIdQueryHandler getByUnitIdHandler,
        GetUnitPersonsByPersonIdQueryHandler getByPersonIdHandler,
        UpdateUnitPersonCommandHandler updateHandler,
        DeleteUnitPersonCommandHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getListHandler = getListHandler;
        _getByIdHandler = getByIdHandler;
        _getByUnitIdHandler = getByUnitIdHandler;
        _getByPersonIdHandler = getByPersonIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUnitPersonCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.Handle(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList(
        CancellationToken cancellationToken)
    {
        var result = await _getListHandler.Handle(
            new GetUnitPersonsQuery(),
            cancellationToken);

        return Ok(result);
    }

 

    [HttpGet("unit/{unitId:guid}")]
    public async Task<IActionResult> GetByUnitId(
        Guid unitId,
        CancellationToken cancellationToken)
    {
        var result = await _getByUnitIdHandler.Handle(
            new GetUnitPersonsByUnitIdQuery(unitId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("person/{personId:guid}")]
    public async Task<IActionResult> GetByPersonId(
        Guid personId,
        CancellationToken cancellationToken)
    {
        var result = await _getByPersonIdHandler.Handle(
            new GetUnitPersonsByPersonIdQuery(personId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
     Guid id,
     CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.Handle(
            new GetUnitPersonByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateUnitPersonCommand command,
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
        var result = await _deleteHandler.Handle(
            new DeleteUnitPersonCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}