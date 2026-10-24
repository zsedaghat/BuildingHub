
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Accounting.Domain.Entities;

public class ChargeAllocation : Entity
{
    public Guid ChargeId { get; private set; }

    public Guid UnitId { get; private set; }

    public decimal Amount { get; private set; }

    private ChargeAllocation()
    {
    }

    private ChargeAllocation(
        Guid chargeId,
        Guid unitId,
        decimal amount)
    {
        ChargeId = chargeId;
        UnitId = unitId;
        Amount = amount;
    }

    public static ChargeAllocation Create(
        Guid chargeId,
        Guid unitId,
        decimal amount)
    {
        if (chargeId == Guid.Empty)
            throw new ArgumentException("ChargeId is required.");

        if (unitId == Guid.Empty)
            throw new ArgumentException("UnitId is required.");

        if (amount <= 0)
            throw new ArgumentException(
                "Allocation amount must be greater than zero.");

        return new ChargeAllocation(
            chargeId,
            unitId,
            amount);
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException(
                "Allocation amount must be greater than zero.");

        Amount = amount;
    }
}
