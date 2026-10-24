
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
}
