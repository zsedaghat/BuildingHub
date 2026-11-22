using BuildingHub.Modules.Community.Domain.Entities;
using BuildingHub.SharedKernel.Common.Pagination;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IUnitRepository : IRepository<Unit>
{
    Task<IReadOnlyList<Unit>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Unit>> GetByFloorIdAsync(
        Guid floorId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Unit>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? buildingId = null,
        Guid? floorId = null,
        CancellationToken cancellationToken = default);

    Task<bool> FloorBelongsToBuildingAsync(
        Guid floorId,
        Guid buildingId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid buildingId,
        string unitNumber,
        Guid? excludeUnitId = null,
        CancellationToken cancellationToken = default);
}
