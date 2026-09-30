using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class UnitRepository
    : Repository<Unit>, IUnitRepository
{
    public UnitRepository(
        CommunityDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Unit>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Units
            .AsNoTracking()
            .Where(x => x.BuildingId == buildingId)
            .OrderBy(x => x.UnitNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Unit>> GetByFloorIdAsync(
        Guid floorId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Units
            .AsNoTracking()
            .Where(x => x.FloorId == floorId)
            .OrderBy(x => x.UnitNumber)
            .ToListAsync(cancellationToken);
    }
}