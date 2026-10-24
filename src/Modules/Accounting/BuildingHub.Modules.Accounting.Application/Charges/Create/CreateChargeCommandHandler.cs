
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.Create;

public sealed class CreateChargeCommandHandler
    : IRequestHandler<CreateChargeCommand, Guid>
{
    private readonly IChargeRepository _chargeRepository;

    public CreateChargeCommandHandler(
        IChargeRepository chargeRepository)
    {
        _chargeRepository = chargeRepository;
    }

    public async Task<Guid> Handle(
        CreateChargeCommand request,
        CancellationToken cancellationToken)
    {
        var charge = Charge.Create(
     request.BuildingId,
     request.Title,
     request.TotalAmount,
     request.CalculationMethod,
     request.Category,
     request.PeriodYear,
     request.PeriodMonth);

        await _chargeRepository.AddAsync(
            charge,
            cancellationToken);

        return charge.Id;
    }
}
