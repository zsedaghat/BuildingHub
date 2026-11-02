using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using BuildingHub.SharedKernel.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class BuildingRepository : Repository<Building>, IBuildingRepository
{
    public BuildingRepository(CommunityDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Building>> GetByCommunityIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Buildings
            .AsNoTracking()
            .Where(x => x.CommunityId == communityId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Building>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Buildings.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var effectivePageNumber = totalPages == 0 ? 1 : Math.Min(pageNumber, totalPages);
        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((effectivePageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Building>(items, effectivePageNumber, pageSize, totalCount);
    }
}
