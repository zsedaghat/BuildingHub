
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.GetAllocations;

public sealed record GetExpenseAllocationsQuery(
    Guid ExpenseId
) : IRequest<IReadOnlyList<ChargeExpense>>;
