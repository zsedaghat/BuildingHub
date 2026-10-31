
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.UpdateAllocation;

public sealed record UpdateExpenseAllocationCommand(
    Guid ExpenseId,
    Guid AllocationId,
    decimal AllocatedAmount
) : IRequest;
