
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.GetAllocationSummary;

public sealed record GetExpenseAllocationSummaryQuery(
    Guid ExpenseId
) : IRequest<ExpenseAllocationSummaryDto>;

public sealed record ExpenseAllocationSummaryDto(
    Guid ExpenseId,
    decimal TotalExpenseAmount,
    decimal TotalAllocatedAmount,
    decimal RemainingAmount
);
