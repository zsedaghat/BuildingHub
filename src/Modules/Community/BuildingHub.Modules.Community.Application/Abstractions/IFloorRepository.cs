using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IFloorRepository : IRepository<Floor>
{
    Task<IReadOnlyList<Floor>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);
}