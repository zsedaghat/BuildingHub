
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetAll;

public sealed class GetChargesQueryHandler
    : IRequestHandler<GetChargesQuery, IReadOnlyList<Charge>>
{
    private readonly IChargeRepository _chargeRepository;

    public GetChargesQueryHandler(
        IChargeRepository chargeRepository)
    {
        _chargeRepository = chargeRepository;
    }

    public async Task<IReadOnlyList<Charge>> Handle(
        GetChargesQuery request,
        CancellationToken cancellationToken)
    {
        return await _chargeRepository.GetByBuildingIdAsync(
            request.BuildingId,
            cancellationToken);
    }
}
