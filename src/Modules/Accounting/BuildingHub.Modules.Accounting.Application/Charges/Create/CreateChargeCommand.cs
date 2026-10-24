
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.Create;

public sealed record CreateChargeCommand(
    Guid BuildingId,
    string Title,
    decimal TotalAmount,
    ChargeCalculationMethod CalculationMethod,
    ChargeCategory Category,
    int PeriodYear,
    int PeriodMonth
) : IRequest<Guid>;
