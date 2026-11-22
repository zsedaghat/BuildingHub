using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using BuildingHub.SharedKernel.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(CommunityDbContext context) : base(context)
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
            .ThenBy(x => x.Id)
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
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Unit>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? buildingId = null,
        Guid? floorId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Unit> query = Context.Units.AsNoTracking();

        if (buildingId.HasValue && buildingId.Value != Guid.Empty)
            query = query.Where(x => x.BuildingId == buildingId.Value);

        if (floorId.HasValue && floorId.Value != Guid.Empty)
            query = query.Where(x => x.FloorId == floorId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);
        var effectivePageNumber = totalPages == 0
            ? 1
            : Math.Min(pageNumber, totalPages);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((effectivePageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Unit>(items, effectivePageNumber, pageSize, totalCount);
    }

    public Task<bool> FloorBelongsToBuildingAsync(
        Guid floorId,
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return Context.Floors.AsNoTracking()
            .AnyAsync(x => x.Id == floorId && x.BuildingId == buildingId, cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid buildingId,
        string unitNumber,
        Guid? excludeUnitId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Units.AsNoTracking()
            .Where(x => x.BuildingId == buildingId && x.UnitNumber == unitNumber);

        if (excludeUnitId.HasValue)
            query = query.Where(x => x.Id != excludeUnitId.Value);

        return query.AnyAsync(cancellationToken);
    }
}
