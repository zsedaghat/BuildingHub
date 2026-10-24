
using BuildingHub.Modules.Accounting.Application.Charges.CalculateUnitCharges;
using BuildingHub.Modules.Accounting.Application.Charges.Create;
using BuildingHub.Modules.Accounting.Application.Charges.GetAll;
using BuildingHub.Modules.Accounting.Application.Charges.GetUnitChargeBalances;
using BuildingHub.Modules.Accounting.Application.Charges.GetUnitCharges;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/buildings/{buildingId:guid}/charges")]
public sealed class ChargesController : ControllerBase
{
    private readonly ISender _sender;

    public ChargesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetByBuildingId(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetChargesQuery(buildingId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid buildingId,
        [FromBody] CreateChargeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateChargeCommand(
            buildingId,
            request.Title,
            request.TotalAmount,
            request.CalculationMethod,
            request.Category,
            request.PeriodYear,
            request.PeriodMonth);

        var chargeId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/buildings/{buildingId}/charges/{chargeId}",
            new { id = chargeId });
    }


    [HttpPost("{chargeId:guid}/calculate")]
    public async Task<IActionResult> Calculate(
        Guid buildingId,
        Guid chargeId,
        CancellationToken cancellationToken)
    {
        var charge = await _sender.Send(
            new GetChargesQuery(buildingId),
            cancellationToken);

        if (!charge.Any(x => x.Id == chargeId))
            return NotFound(new { message = "Charge was not found in this building." });

        var count = await _sender.Send(
            new CalculateUnitChargesCommand(chargeId),
            cancellationToken);

        return Ok(new
        {
            chargeId,
            calculatedUnits = count,
            message = "Unit charges calculated successfully."
        });
    }


    [HttpGet("{chargeId:guid}/units")]
    public async Task<IActionResult> GetUnitCharges(
        Guid buildingId,
        Guid chargeId,
        CancellationToken cancellationToken)
    {
        var charges = await _sender.Send(
            new GetChargesQuery(buildingId),
            cancellationToken);

        if (!charges.Any(x => x.Id == chargeId))
            return NotFound(new { message = "Charge was not found in this building." });

        var result = await _sender.Send(
         new GetUnitChargesQuery(buildingId, chargeId),
            cancellationToken);

        return Ok(result);
    }


    [HttpGet("{chargeId:guid}/balances")]
    public async Task<IActionResult> GetUnitChargeBalances(
        Guid buildingId,
        Guid chargeId,
        CancellationToken cancellationToken)
    {
        var charges = await _sender.Send(
            new GetChargesQuery(buildingId),
            cancellationToken);

        if (!charges.Any(x => x.Id == chargeId))
        {
            return NotFound(new
            {
                message = "Charge was not found in this building."
            });
        }

        var balances = await _sender.Send(
            new GetUnitChargeBalancesQuery(buildingId, chargeId),
            cancellationToken);

        return Ok(balances);
    }



}

public sealed record CreateChargeRequest(
    string Title,
    decimal TotalAmount,
    BuildingHub.Modules.Accounting.Domain.Entities.ChargeCalculationMethod CalculationMethod,
    BuildingHub.Modules.Accounting.Domain.Entities.ChargeCategory Category,
    int PeriodYear,
    int PeriodMonth);



