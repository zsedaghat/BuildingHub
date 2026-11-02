using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using BuildingHub.SharedKernel.Common.Pagination;
using CommunityEntity = BuildingHub.Modules.Community.Domain.Entities.Community;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class CommunityRepository
    : Repository<CommunityEntity>, ICommunityRepository
{
    public CommunityRepository(CommunityDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<CommunityEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<CommunityEntity>().AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var effectivePageNumber = totalPages == 0 ? 1 : Math.Min(pageNumber, totalPages);
        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Skip((effectivePageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CommunityEntity>(items, effectivePageNumber, pageSize, totalCount);
    }
}
