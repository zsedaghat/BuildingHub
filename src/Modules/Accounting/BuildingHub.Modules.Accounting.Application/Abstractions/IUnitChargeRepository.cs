
using BuildingHub.Modules.Accounting.Domain.Entities;

namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public interface IUnitChargeRepository : IRepository<UnitCharge>
{
    Task<IReadOnlyList<UnitCharge>> GetByChargeIdAsync(
        Guid chargeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UnitCharge>> GetByUnitIdAsync(
        Guid unitId,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<UnitCharge> unitCharges,
        CancellationToken cancellationToken = default);
}
