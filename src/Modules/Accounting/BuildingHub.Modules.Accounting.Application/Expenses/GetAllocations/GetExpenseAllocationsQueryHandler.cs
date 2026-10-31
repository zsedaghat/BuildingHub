
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.GetAllocations;

public sealed class GetExpenseAllocationsQueryHandler
    : IRequestHandler<GetExpenseAllocationsQuery, IReadOnlyList<ChargeExpense>>
{
    private readonly IChargeExpenseRepository _chargeExpenseRepository;

    public GetExpenseAllocationsQueryHandler(
        IChargeExpenseRepository chargeExpenseRepository)
    {
        _chargeExpenseRepository = chargeExpenseRepository;
    }

    public async Task<IReadOnlyList<ChargeExpense>> Handle(
        GetExpenseAllocationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _chargeExpenseRepository.GetByExpenseIdAsync(
            request.ExpenseId,
            cancellationToken);
    }
}
