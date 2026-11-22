
using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IFloorRepository : IRepository<Floor>
{
    Task<IReadOnlyList<Floor>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid buildingId,
        int floorNumber,
        Guid? excludeFloorId = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Floor>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? buildingId = null,
        CancellationToken cancellationToken = default);
}
