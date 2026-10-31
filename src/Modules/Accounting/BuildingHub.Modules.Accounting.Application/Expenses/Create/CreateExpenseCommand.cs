
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.Create;

public sealed record CreateExpenseCommand(
    Guid BuildingId,
    string Title,
    decimal Amount,
    DateOnly ExpenseDate,
    string? Description
) : IRequest<Guid>;
