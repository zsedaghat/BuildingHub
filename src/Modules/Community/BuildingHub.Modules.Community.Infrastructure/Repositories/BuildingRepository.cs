using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.Modules.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingHub.Modules.Community.Infrastructure.Repositories;

public sealed class BuildingRepository
    : Repository<Building>, IBuildingRepository
{
    public BuildingRepository(
        CommunityDbContext context)
        : base(context)
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
            .ToListAsync(cancellationToken);
    }
}