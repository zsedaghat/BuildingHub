
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.GetAllocationSummary;

public sealed class GetExpenseAllocationSummaryQueryHandler
    : IRequestHandler<GetExpenseAllocationSummaryQuery, ExpenseAllocationSummaryDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IChargeExpenseRepository _chargeExpenseRepository;

    public GetExpenseAllocationSummaryQueryHandler(
        IExpenseRepository expenseRepository,
        IChargeExpenseRepository chargeExpenseRepository)
    {
        _expenseRepository = expenseRepository;
        _chargeExpenseRepository = chargeExpenseRepository;
    }

    public async Task<ExpenseAllocationSummaryDto> Handle(
        GetExpenseAllocationSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdAsync(
            request.ExpenseId,
            cancellationToken);

        if (expense is null)
        {
            throw new InvalidOperationException("Expense was not found.");
        }

        var totalAllocated =
            await _chargeExpenseRepository.GetTotalAllocatedAmountByExpenseIdAsync(
                request.ExpenseId,
                cancellationToken);

        var remainingAmount = expense.Amount - totalAllocated;

        return new ExpenseAllocationSummaryDto(
            expense.Id,
            expense.Amount,
            totalAllocated,
            remainingAmount);
    }
}
