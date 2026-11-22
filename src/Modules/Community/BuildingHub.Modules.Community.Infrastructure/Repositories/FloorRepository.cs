using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using BuildingHub.SharedKernel.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class FloorRepository : Repository<Floor>, IFloorRepository
{
    public FloorRepository(CommunityDbContext context) : base(context)
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
            .ThenByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Floor>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? buildingId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Floors.AsNoTracking();

        if (buildingId.HasValue)
            query = query.Where(x => x.BuildingId == buildingId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (totalPages > 0)
            pageNumber = Math.Min(pageNumber, totalPages);
        else
            pageNumber = 1;

        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Floor>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

    public Task<bool> ExistsAsync(
        Guid buildingId,
        int floorNumber,
        Guid? excludeFloorId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Floors
            .AsNoTracking()
            .Where(x => x.BuildingId == buildingId && x.FloorNumber == floorNumber);

        if (excludeFloorId.HasValue)
            query = query.Where(x => x.Id != excludeFloorId.Value);

        return query.AnyAsync(cancellationToken);
    }
}
