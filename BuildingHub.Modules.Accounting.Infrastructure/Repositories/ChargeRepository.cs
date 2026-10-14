using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using BuildingHub.Modules.Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Accounting.Infrastructure.Repositories;

public sealed class ChargeRepository
    : Repository<Charge>, IChargeRepository
{
    public ChargeRepository(AccountingDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Charge>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Charges
            .AsNoTracking()
            .Where(x => x.BuildingId == buildingId)
            .OrderByDescending(x => x.PeriodYear)
            .ThenByDescending(x => x.PeriodMonth)
            .ToListAsync(cancellationToken);
    }
}