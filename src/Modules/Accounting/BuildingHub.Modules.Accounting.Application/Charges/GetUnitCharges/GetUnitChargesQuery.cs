
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetUnitCharges;

public sealed record GetUnitChargesQuery(
    Guid BuildingId,
    Guid ChargeId
) : IRequest<IReadOnlyList<UnitChargeDto>>;
