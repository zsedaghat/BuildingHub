
using BuildingHub.Modules.Accounting.Application.Expenses.AllocateToCharge;
using BuildingHub.Modules.Accounting.Application.Expenses.Create;
using BuildingHub.Modules.Accounting.Application.Expenses.DeleteAllocation;
using BuildingHub.Modules.Accounting.Application.Expenses.GetAllocations;
using BuildingHub.Modules.Accounting.Application.Expenses.GetAllocationSummary;
using BuildingHub.Modules.Accounting.Application.Expenses.GetByBuilding;
using BuildingHub.Modules.Accounting.Application.Expenses.UpdateAllocation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildingHub.Api.Controllers;

[ApiController]
[Route("api/buildings/{buildingId:guid}/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private readonly ISender _sender;

    public ExpensesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid buildingId,
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateExpenseCommand(
            buildingId,
            request.Title,
            request.Amount,
            request.ExpenseDate,
            request.Description);

        var expenseId = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/buildings/{buildingId}/expenses/{expenseId}",
            new { id = expenseId });
    }

    [HttpGet]
    public async Task<IActionResult> GetByBuilding(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(
            new GetExpensesByBuildingQuery(buildingId),
            cancellationToken);

        return Ok(expenses);
    }

    [HttpPost("{expenseId:guid}/allocations")]
    public async Task<IActionResult> AllocateToCharge(
        Guid buildingId,
        Guid expenseId,
        [FromBody] AllocateExpenseToChargeRequest request,
        CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(
            new GetExpensesByBuildingQuery(buildingId),
            cancellationToken);

        if (!expenses.Any(x => x.Id == expenseId))
        {
            return NotFound(new
            {
                message = "Expense was not found in this building."
            });
        }

        var allocationId = await _sender.Send(
            new AllocateExpenseToChargeCommand(
                expenseId,
                request.ChargeId,
                request.AllocatedAmount),
            cancellationToken);

        return Created(
            $"/api/buildings/{buildingId}/expenses/{expenseId}/allocations/{allocationId}",
            new { id = allocationId });
    }


    [HttpGet("{expenseId:guid}/allocations")]
    public async Task<IActionResult> GetAllocations(
        Guid buildingId,
        Guid expenseId,
        CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(
            new GetExpensesByBuildingQuery(buildingId),
            cancellationToken);

        if (!expenses.Any(x => x.Id == expenseId))
        {
            return NotFound(new
            {
                message = "Expense was not found in this building."
            });
        }

        var allocations = await _sender.Send(
            new GetExpenseAllocationsQuery(expenseId),
            cancellationToken);

        return Ok(allocations.Select(x => new
        {
            x.Id,
            x.ExpenseId,
            x.ChargeId,
            x.AllocatedAmount,
            x.CreatedAt
        }));
    }


    [HttpGet("{expenseId:guid}/allocation-summary")]
    public async Task<IActionResult> GetAllocationSummary(
        Guid buildingId,
        Guid expenseId,
        CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(
            new GetExpensesByBuildingQuery(buildingId),
            cancellationToken);

        if (!expenses.Any(x => x.Id == expenseId))
        {
            return NotFound(new
            {
                message = "Expense was not found in this building."
            });
        }

        var summary = await _sender.Send(
            new GetExpenseAllocationSummaryQuery(expenseId),
            cancellationToken);

        return Ok(summary);
    }


    [HttpPut("{expenseId:guid}/allocations/{allocationId:guid}")]
    public async Task<IActionResult> UpdateAllocation(
        Guid buildingId,
        Guid expenseId,
        Guid allocationId,
        [FromBody] UpdateExpenseAllocationRequest request,
        CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(
            new GetExpensesByBuildingQuery(buildingId),
            cancellationToken);

        if (!expenses.Any(x => x.Id == expenseId))
        {
            return NotFound(new
            {
                message = "Expense was not found in this building."
            });
        }

        await _sender.Send(
            new UpdateExpenseAllocationCommand(
                expenseId,
                allocationId,
                request.AllocatedAmount),
            cancellationToken);

        return NoContent();
    }


    [HttpDelete("{expenseId:guid}/allocations/{allocationId:guid}")]
    public async Task<IActionResult> DeleteAllocation(
        Guid buildingId,
        Guid expenseId,
        Guid allocationId,
        CancellationToken cancellationToken)
    {
        var expenses = await _sender.Send(
            new GetExpensesByBuildingQuery(buildingId),
            cancellationToken);

        if (!expenses.Any(x => x.Id == expenseId))
        {
            return NotFound(new
            {
                message = "Expense was not found in this building."
            });
        }

        await _sender.Send(
            new DeleteExpenseAllocationCommand(expenseId, allocationId),
            cancellationToken);

        return NoContent();
    }



}

public sealed record CreateExpenseRequest(
    string Title,
    decimal Amount,
    DateOnly ExpenseDate,
    string? Description);

public sealed record AllocateExpenseToChargeRequest(
    Guid ChargeId,
    decimal AllocatedAmount);

public sealed record UpdateExpenseAllocationRequest(
    decimal AllocatedAmount);
