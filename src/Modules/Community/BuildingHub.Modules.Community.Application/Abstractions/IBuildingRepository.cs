using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IBuildingRepository : IRepository<Building>
{
    Task<IReadOnlyList<Building>> GetByCommunityIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Building>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
