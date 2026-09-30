using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class FloorRepository
    : Repository<Floor>, IFloorRepository
{
    public FloorRepository(
        CommunityDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Floor>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Floors
            .AsNoTracking()
            .Where(x => x.BuildingId == buildingId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.FloorNumber)
            .ToListAsync(cancellationToken);
    }
}