
namespace BuildingHub.Modules.Accounting.Application.Charges.GetBuildingUnitBalances;

public sealed record BuildingUnitBalanceDto(
    Guid UnitId,
    string UnitNumber,
    decimal TotalChargeAmount,
    decimal TotalPaidAmount,
    decimal RemainingAmount
);
