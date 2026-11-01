
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetBuildingUnitBalances;

public sealed record GetBuildingUnitBalancesQuery(
    Guid BuildingId
) : IRequest<IReadOnlyList<BuildingUnitBalanceDto>>;
