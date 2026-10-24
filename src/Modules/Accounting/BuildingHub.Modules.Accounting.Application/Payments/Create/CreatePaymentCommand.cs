
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Payments.Create;

public sealed record CreatePaymentCommand(
    Guid UnitChargeId,
    decimal Amount,
    DateTime PaidAt,
    string? TrackingNumber
) : IRequest<Guid>;
