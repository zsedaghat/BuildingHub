
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.DeleteAllocation;

public sealed class DeleteExpenseAllocationCommandHandler
    : IRequestHandler<DeleteExpenseAllocationCommand>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IChargeExpenseRepository _chargeExpenseRepository;

    public DeleteExpenseAllocationCommandHandler(
        IExpenseRepository expenseRepository,
        IChargeExpenseRepository chargeExpenseRepository)
    {
        _expenseRepository = expenseRepository;
        _chargeExpenseRepository = chargeExpenseRepository;
    }

    public async Task Handle(
        DeleteExpenseAllocationCommand request,
        CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdAsync(
            request.ExpenseId,
            cancellationToken);

        if (expense is null)
        {
            throw new InvalidOperationException("Expense was not found.");
        }

        var allocation = await _chargeExpenseRepository.GetByIdAsync(
            request.AllocationId,
            cancellationToken);

        if (allocation is null ||
            allocation.ExpenseId != request.ExpenseId)
        {
            throw new InvalidOperationException(
                "Allocation was not found for this expense.");
        }

        await _chargeExpenseRepository.DeleteAsync(
            allocation,
            cancellationToken);
    }
}
