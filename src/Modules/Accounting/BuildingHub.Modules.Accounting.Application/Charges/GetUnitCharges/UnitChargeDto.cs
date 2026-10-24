
namespace BuildingHub.Modules.Accounting.Application.Charges.GetUnitCharges;

public sealed record UnitChargeDto(
    Guid UnitId,
    string UnitNumber,
    decimal? Area,
    decimal Amount,
    DateTime CreatedAt
);
