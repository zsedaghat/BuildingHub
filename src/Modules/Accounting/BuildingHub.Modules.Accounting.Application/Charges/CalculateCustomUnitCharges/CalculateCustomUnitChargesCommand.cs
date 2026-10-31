
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.CalculateCustomUnitCharges;

public sealed record CustomUnitAmount(
    Guid UnitId,
    decimal Amount
);

public sealed record CalculateCustomUnitChargesCommand(
    Guid ChargeId,
    List<CustomUnitAmount> Units
) : IRequest<int>;
