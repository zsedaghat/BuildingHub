
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using BuildingHub.Modules.Accounting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Accounting.Infrastructure.Persistence;

public sealed class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AccountingDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Payment>> GetByUnitChargeIdAsync(
        Guid unitChargeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .AsNoTracking()
            .Where(payment => payment.UnitChargeId == unitChargeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByUnitChargeIdsAsync(
        IReadOnlyCollection<Guid> unitChargeIds,
        CancellationToken cancellationToken = default)
    {
        if (unitChargeIds.Count == 0)
            return Array.Empty<Payment>();

        return await Context.Set<Payment>()
            .AsNoTracking()
            .Where(payment => unitChargeIds.Contains(payment.UnitChargeId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> CreatePaymentWithBalanceCheckAsync(
        Guid unitChargeId,
        decimal amount,
        DateTime paidAt,
        string? trackingNumber,
        CancellationToken cancellationToken = default)
    {
        if (unitChargeId == Guid.Empty)
            throw new ArgumentException("UnitChargeId is required.");

        if (amount <= 0)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");

        await using var transaction =
            await Context.Database.BeginTransactionAsync(cancellationToken);

        var unitCharge = await Context.UnitCharges
            .FromSqlInterpolated(
                $"SELECT * FROM \"UnitCharges\" WHERE \"Id\" = {unitChargeId} FOR UPDATE")
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (unitCharge is null)
            throw new InvalidOperationException(
                "Unit charge was not found.");

        var paidAmount = await Context.Payments
            .Where(payment => payment.UnitChargeId == unitChargeId)
            .SumAsync(payment => (decimal?)payment.Amount, cancellationToken)
            ?? 0m;

        var remainingAmount = unitCharge.Amount - paidAmount;

        if (remainingAmount <= 0)
            throw new InvalidOperationException(
                "This unit charge has already been fully paid.");

        if (amount > remainingAmount)
            throw new InvalidOperationException(
                $"Payment amount exceeds the remaining balance of {remainingAmount}.");

        var payment = Payment.Create(
            unitChargeId,
            amount,
            paidAt,
            trackingNumber);

        await Context.Payments.AddAsync(payment, cancellationToken);

        await Context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return payment.Id;
    }
}
