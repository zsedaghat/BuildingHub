using BuildingHub.Modules.Community.Application.Persons.Create;
using BuildingHub.Modules.Community.Application.Persons.Delete;
using BuildingHub.Modules.Community.Application.Persons.GetById;
using BuildingHub.Modules.Community.Application.Persons.GetByMobile;
using BuildingHub.Modules.Community.Application.Persons.GetList;
using BuildingHub.Modules.Community.Application.Persons.Update;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/persons")]
public class PersonsController : ControllerBase
{
    private readonly CreatePersonCommandHandler _createHandler;
    private readonly GetPersonsQueryHandler _getListHandler;
    private readonly GetPersonByIdQueryHandler _getByIdHandler;
    private readonly UpdatePersonCommandHandler _updateHandler;
    private readonly DeletePersonCommandHandler _deleteHandler;
    private readonly GetPersonByMobileQueryHandler _getByMobileHandler;

    public PersonsController(
        CreatePersonCommandHandler createHandler,
        GetPersonsQueryHandler getListHandler,
        GetPersonByIdQueryHandler getByIdHandler,
        UpdatePersonCommandHandler updateHandler,
        DeletePersonCommandHandler deleteHandler,
         GetPersonByMobileQueryHandler getByMobileHandler)
    {
        _createHandler = createHandler;
        _getListHandler = getListHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePersonCommand command,
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
            new GetPersonsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("mobile/{mobile}")]
    public async Task<IActionResult> GetByMobile(
    string mobile,
    CancellationToken cancellationToken)
    {
        var result = await _getByMobileHandler.Handle(
            new GetPersonByMobileQuery(mobile),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.Handle(
            new GetPersonByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePersonCommand command,
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
            new DeletePersonCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}