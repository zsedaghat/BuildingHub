using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Abstractions;

public interface IUnitRepository : IRepository<Unit>
{
    Task<IReadOnlyList<Unit>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Unit>> GetByFloorIdAsync(
        Guid floorId,
        CancellationToken cancellationToken = default);
}