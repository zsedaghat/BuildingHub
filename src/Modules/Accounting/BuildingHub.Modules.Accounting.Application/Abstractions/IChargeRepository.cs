using BuildingHub.Modules.Accounting.Domain.Entities;

namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public interface IChargeRepository : IRepository<Charge>
{
    Task<IReadOnlyList<Charge>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);
}