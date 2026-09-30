using BuildingHub.Modules.Community.Application.Communities;
using BuildingHub.Modules.Community.Application.Communities.Create;
using BuildingHub.Modules.Community.Application.Communities.GetList;
using BuildingHub.Modules.Community.Application.Communities.GetById;
using BuildingHub.Modules.Community.Application.Communities.Update;
using BuildingHub.Modules.Community.Application.Communities.Delete;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/communities")]
public class CommunitiesController : ControllerBase
{
    private readonly CreateCommunityCommandHandler _createHandler;
    private readonly GetCommunitiesQueryHandler _getCommunitiesHandler;
    private readonly GetCommunityByIdQueryHandler _getCommunityByIdHandler;
    private readonly UpdateCommunityCommandHandler _updateHandler;
    private readonly DeleteCommunityCommandHandler _deleteHandler;

    public CommunitiesController(
        CreateCommunityCommandHandler createHandler,
        GetCommunitiesQueryHandler getCommunitiesHandler,
        GetCommunityByIdQueryHandler getCommunityByIdHandler,
        UpdateCommunityCommandHandler updateHandler,
        DeleteCommunityCommandHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getCommunitiesHandler = getCommunitiesHandler;
        _getCommunityByIdHandler = getCommunityByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCommunityCommand command,
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
        var result = await _getCommunitiesHandler.Handle(
            new GetCommunitiesQuery(),
            cancellationToken);

        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getCommunityByIdHandler.Handle(
            new GetCommunityByIdQuery(id),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // UPDATE
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCommunityCommand command,
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
            new DeleteCommunityCommand(id),
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}