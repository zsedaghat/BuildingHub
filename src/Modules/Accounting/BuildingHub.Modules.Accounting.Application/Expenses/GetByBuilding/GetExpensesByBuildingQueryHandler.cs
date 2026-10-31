
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.GetByBuilding;

public sealed class GetExpensesByBuildingQueryHandler
    : IRequestHandler<
        GetExpensesByBuildingQuery,
        IReadOnlyList<Expense>>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpensesByBuildingQueryHandler(
        IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<IReadOnlyList<Expense>> Handle(
        GetExpensesByBuildingQuery request,
        CancellationToken cancellationToken)
    {
        return await _expenseRepository.GetByBuildingIdAsync(
            request.BuildingId,
            cancellationToken);
    }
}
