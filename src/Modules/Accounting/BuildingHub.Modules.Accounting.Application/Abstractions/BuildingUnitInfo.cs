
namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public sealed record BuildingUnitInfo(
    Guid UnitId,
    string UnitNumber,
    decimal? Area,
    int ResidentCount
);
