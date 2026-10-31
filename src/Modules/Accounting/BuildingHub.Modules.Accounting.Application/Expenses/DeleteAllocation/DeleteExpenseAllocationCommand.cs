
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.DeleteAllocation;

public sealed record DeleteExpenseAllocationCommand(
    Guid ExpenseId,
    Guid AllocationId
) : IRequest;
