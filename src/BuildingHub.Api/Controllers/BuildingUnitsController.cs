
using BuildingHub.Modules.Community.Application.Units.GetActiveByBuildingId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/buildings/{buildingId:guid}/units")]
public sealed class BuildingUnitsController : ControllerBase
{
    private readonly ISender _sender;

    public BuildingUnitsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveUnits(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetActiveUnitsByBuildingIdQuery(buildingId),
            cancellationToken);

        return Ok(result);
    }
}
