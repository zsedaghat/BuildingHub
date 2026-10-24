
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetUnitCharges;

public sealed class GetUnitChargesQueryHandler
    : IRequestHandler<GetUnitChargesQuery, IReadOnlyList<UnitChargeDto>>
{
    private readonly IUnitChargeRepository _unitChargeRepository;
    private readonly IBuildingUnitProvider _unitProvider;

    public GetUnitChargesQueryHandler(
        IUnitChargeRepository unitChargeRepository,
        IBuildingUnitProvider unitProvider)
    {
        _unitChargeRepository = unitChargeRepository;
        _unitProvider = unitProvider;
    }

    public async Task<IReadOnlyList<UnitChargeDto>> Handle(
        GetUnitChargesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.BuildingId == Guid.Empty ||
            request.ChargeId == Guid.Empty)
        {
            throw new ArgumentException(
                "BuildingId and ChargeId are required.");
        }

        var unitCharges =
            await _unitChargeRepository.GetByChargeIdAsync(
                request.ChargeId,
                cancellationToken);

        var units = await _unitProvider.GetActiveUnitsAsync(
            request.BuildingId,
            cancellationToken);

        var unitsById = units.ToDictionary(x => x.UnitId);

        return unitCharges.Select(charge =>
        {
            if (unitsById.TryGetValue(charge.UnitId, out var unit))
            {
                return new UnitChargeDto(
                    charge.UnitId,
                    unit.UnitNumber,
                    unit.Area,
                    charge.Amount,
                    charge.CreatedAt);
            }

            return new UnitChargeDto(
                charge.UnitId,
                "واحد غیرفعال یا یافت‌نشده",
                null,
                charge.Amount,
                charge.CreatedAt);
        }).ToList();
    }
}
