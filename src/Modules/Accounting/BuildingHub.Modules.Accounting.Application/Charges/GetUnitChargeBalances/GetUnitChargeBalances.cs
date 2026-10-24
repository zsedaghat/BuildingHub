
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetUnitChargeBalances;

public sealed record GetUnitChargeBalancesQuery(
    Guid BuildingId,
    Guid ChargeId
) : IRequest<IReadOnlyList<UnitChargeBalanceDto>>;
