
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Expenses.AllocateToCharge;

public sealed class AllocateExpenseToChargeCommandHandler
    : IRequestHandler<AllocateExpenseToChargeCommand, Guid>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IChargeExpenseRepository _chargeExpenseRepository;

    public AllocateExpenseToChargeCommandHandler(
        IExpenseRepository expenseRepository,
        IChargeRepository chargeRepository,
        IChargeExpenseRepository chargeExpenseRepository)
    {
        _expenseRepository = expenseRepository;
        _chargeRepository = chargeRepository;
        _chargeExpenseRepository = chargeExpenseRepository;
    }

    public async Task<Guid> Handle(
        AllocateExpenseToChargeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.AllocatedAmount <= 0)
        {
            throw new ArgumentException(
                "Allocated amount must be greater than zero.");
        }

        var expense = await _expenseRepository.GetByIdAsync(
            request.ExpenseId,
            cancellationToken);

        if (expense is null)
        {
            throw new InvalidOperationException(
                "Expense was not found.");
        }

        var charge = await _chargeRepository.GetByIdAsync(
            request.ChargeId,
            cancellationToken);

        if (charge is null)
        {
            throw new InvalidOperationException(
                "Charge was not found.");
        }

        if (expense.BuildingId != charge.BuildingId)
        {
            throw new InvalidOperationException(
                "Expense and charge must belong to the same building.");
        }

        var alreadyExists = await _chargeExpenseRepository.ExistsAsync(
            request.ExpenseId,
            request.ChargeId,
            cancellationToken);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "This expense is already allocated to this charge.");
        }

        var chargeExpense = ChargeExpense.Create(
            request.ExpenseId,
            request.ChargeId,
            request.AllocatedAmount);

        await _chargeExpenseRepository.AddAllocationSafelyAsync(
            chargeExpense,
            cancellationToken);

        return chargeExpense.Id;
    }
}
