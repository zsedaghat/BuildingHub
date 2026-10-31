
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.GetByBuilding;

public sealed record GetExpensesByBuildingQuery(
    Guid BuildingId
) : IRequest<IReadOnlyList<Expense>>;
