
using BuildingHub.SharedKernel.Domain;

namespace BuildingHub.Modules.Accounting.Domain.Entities;

public class Payment : Entity
{
    public Guid UnitChargeId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime PaidAt { get; private set; }

    public string? TrackingNumber { get; private set; }

    private Payment()
    {
    }

    private Payment(
        Guid unitChargeId,
        decimal amount,
        DateTime paidAt,
        string? trackingNumber)
    {
        UnitChargeId = unitChargeId;
        Amount = amount;
        PaidAt = paidAt;
        TrackingNumber = trackingNumber;
    }

    public static Payment Create(
        Guid unitChargeId,
        decimal amount,
        DateTime paidAt,
        string? trackingNumber = null)
    {
        if (unitChargeId == Guid.Empty)
            throw new ArgumentException("UnitChargeId is required.");

        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        if (paidAt == default)
            throw new ArgumentException("Payment date is required.");

        return new Payment(
            unitChargeId,
            amount,
            paidAt,
            trackingNumber);
    }
}
