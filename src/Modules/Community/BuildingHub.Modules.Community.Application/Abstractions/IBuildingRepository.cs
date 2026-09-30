using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IBuildingRepository : IRepository<Building>
{
    Task<IReadOnlyList<Building>> GetByCommunityIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default);
}