
namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public interface IBuildingUnitProvider
{
    Task<IReadOnlyList<BuildingUnitInfo>> GetActiveUnitsAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);
}
