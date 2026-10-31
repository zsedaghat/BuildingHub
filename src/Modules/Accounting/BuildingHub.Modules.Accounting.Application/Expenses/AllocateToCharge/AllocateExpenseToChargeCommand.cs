
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.AllocateToCharge;

public sealed record AllocateExpenseToChargeCommand(
    Guid ExpenseId,
    Guid ChargeId,
    decimal AllocatedAmount
) : IRequest<Guid>;
