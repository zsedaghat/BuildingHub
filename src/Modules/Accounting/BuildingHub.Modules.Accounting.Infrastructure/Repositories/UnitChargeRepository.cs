
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using BuildingHub.Modules.Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Accounting.Infrastructure.Repositories;

public sealed class UnitChargeRepository
    : Repository<UnitCharge>, IUnitChargeRepository
{
    public UnitChargeRepository(AccountingDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<UnitCharge>> GetByChargeIdAsync(
        Guid chargeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.UnitCharges
            .AsNoTracking()
            .Where(x => x.ChargeId == chargeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnitCharge>> GetByUnitIdAsync(
        Guid unitId,
        CancellationToken cancellationToken = default)
    {
        return await Context.UnitCharges
            .AsNoTracking()
            .Where(x => x.UnitId == unitId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<UnitCharge> unitCharges,
        CancellationToken cancellationToken = default)
    {
        await Context.UnitCharges.AddRangeAsync(
            unitCharges,
            cancellationToken);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnitChargeBalanceSummary>>
        GetBuildingBalanceSummariesAsync(
            Guid buildingId,
            CancellationToken cancellationToken = default)
    {
        var chargeAmounts = await (
            from unitCharge in Context.UnitCharges.AsNoTracking()
            join charge in Context.Charges.AsNoTracking()
                on unitCharge.ChargeId equals charge.Id
            where charge.BuildingId == buildingId
            group unitCharge by unitCharge.UnitId
            into unitGroup
            select new
            {
                UnitId = unitGroup.Key,
                TotalChargeAmount = unitGroup.Sum(x => x.Amount)
            })
            .ToListAsync(cancellationToken);

        var paidAmounts = await (
            from payment in Context.Payments.AsNoTracking()
            join unitCharge in Context.UnitCharges.AsNoTracking()
                on payment.UnitChargeId equals unitCharge.Id
            join charge in Context.Charges.AsNoTracking()
                on unitCharge.ChargeId equals charge.Id
            where charge.BuildingId == buildingId
            group payment by unitCharge.UnitId
            into unitGroup
            select new
            {
                UnitId = unitGroup.Key,
                TotalPaidAmount = unitGroup.Sum(x => x.Amount)
            })
            .ToListAsync(cancellationToken);

        var paidAmountsByUnitId = paidAmounts.ToDictionary(
            x => x.UnitId,
            x => x.TotalPaidAmount);

        return chargeAmounts
            .Select(x => new UnitChargeBalanceSummary(
                x.UnitId,
                x.TotalChargeAmount,
                paidAmountsByUnitId.TryGetValue(
                    x.UnitId,
                    out var paidAmount)
                    ? paidAmount
                    : 0m))
            .ToList();
    }
}
