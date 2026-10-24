
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Accounting.Domain.Entities;

public class UnitCharge : Entity
{
    public Guid ChargeId { get; private set; }

    public Guid UnitId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private UnitCharge()
    {
    }

    private UnitCharge(
        Guid chargeId,
        Guid unitId,
        decimal amount)
    {
        ChargeId = chargeId;
        UnitId = unitId;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }

    public static UnitCharge Create(
        Guid chargeId,
        Guid unitId,
        decimal amount)
    {
        if (chargeId == Guid.Empty)
            throw new ArgumentException(
                "ChargeId is required.");

        if (unitId == Guid.Empty)
            throw new ArgumentException(
                "UnitId is required.");

        if (amount < 0)
            throw new ArgumentException(
                "Amount cannot be negative.");

        return new UnitCharge(
            chargeId,
            unitId,
            amount);
    }

    public void UpdateAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException(
                "Amount cannot be negative.");

        Amount = amount;
    }
}
