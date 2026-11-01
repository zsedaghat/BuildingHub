
namespace BuildingHub.Modules.Accounting.Application.Abstractions;

public sealed record UnitChargeBalanceSummary(
    Guid UnitId,
    decimal TotalChargeAmount,
    decimal TotalPaidAmount
);
