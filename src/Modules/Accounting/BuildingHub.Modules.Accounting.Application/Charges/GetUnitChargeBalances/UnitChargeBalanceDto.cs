
namespace BuildingHub.Modules.Accounting.Application.Charges.GetUnitChargeBalances;

public sealed record UnitChargeBalanceDto(
    Guid UnitChargeId,
    Guid UnitId,
    string UnitNumber,
    decimal ChargeAmount,
    decimal PaidAmount,
    decimal RemainingAmount
);
