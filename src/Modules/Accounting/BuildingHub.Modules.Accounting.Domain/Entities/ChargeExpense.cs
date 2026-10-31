
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Accounting.Domain.Entities;

public class ChargeExpense : Entity
{
    public Guid ExpenseId { get; private set; }

    public Guid ChargeId { get; private set; }

    public decimal AllocatedAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private ChargeExpense()
    {
    }

    private ChargeExpense(
        Guid expenseId,
        Guid chargeId,
        decimal allocatedAmount)
    {
        ExpenseId = expenseId;
        ChargeId = chargeId;
        AllocatedAmount = allocatedAmount;
        CreatedAt = DateTime.UtcNow;
    }

    public static ChargeExpense Create(
        Guid expenseId,
        Guid chargeId,
        decimal allocatedAmount)
    {
        if (expenseId == Guid.Empty)
            throw new ArgumentException(
                "ExpenseId is required.");

        if (chargeId == Guid.Empty)
            throw new ArgumentException(
                "ChargeId is required.");

        if (allocatedAmount <= 0)
            throw new ArgumentException(
                "Allocated amount must be greater than zero.");

        return new ChargeExpense(
            expenseId,
            chargeId,
            allocatedAmount);
    }

    public void UpdateAllocatedAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException(
                "Allocated amount must be greater than zero.");

        AllocatedAmount = amount;
    }
}
