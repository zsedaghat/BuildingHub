
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.CalculateUnitCharges;

public sealed record CalculateUnitChargesCommand(Guid ChargeId)
    : IRequest<int>;
