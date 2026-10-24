
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetUnitChargeBalances;

public sealed class GetUnitChargeBalancesQueryHandler
    : IRequestHandler<GetUnitChargeBalancesQuery, IReadOnlyList<UnitChargeBalanceDto>>
{
    private readonly IUnitChargeRepository _unitChargeRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBuildingUnitProvider _unitProvider;

    public GetUnitChargeBalancesQueryHandler(
        IUnitChargeRepository unitChargeRepository,
        IPaymentRepository paymentRepository,
        IBuildingUnitProvider unitProvider)
    {
        _unitChargeRepository = unitChargeRepository;
        _paymentRepository = paymentRepository;
        _unitProvider = unitProvider;
    }

    public async Task<IReadOnlyList<UnitChargeBalanceDto>> Handle(
        GetUnitChargeBalancesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.BuildingId == Guid.Empty ||
            request.ChargeId == Guid.Empty)
        {
            throw new ArgumentException(
                "BuildingId and ChargeId are required.");
        }

        var unitCharges = await _unitChargeRepository.GetByChargeIdAsync(
            request.ChargeId,
            cancellationToken);

        var units = await _unitProvider.GetActiveUnitsAsync(
            request.BuildingId,
            cancellationToken);

        var unitsById = units.ToDictionary(x => x.UnitId);

        // Fetch all payments in one query instead of querying per unit.
        var unitChargeIds = unitCharges
            .Select(x => x.Id)
            .ToList();

        var payments = await _paymentRepository.GetByUnitChargeIdsAsync(
            unitChargeIds,
            cancellationToken);

        var paidAmountsByUnitChargeId = payments
            .GroupBy(payment => payment.UnitChargeId)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(payment => payment.Amount));

        var result = new List<UnitChargeBalanceDto>();

        foreach (var unitCharge in unitCharges)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var paidAmount = paidAmountsByUnitChargeId.TryGetValue(
                unitCharge.Id,
                out var totalPaid)
                ? totalPaid
                : 0m;

            var unitNumber = unitsById.TryGetValue(
                unitCharge.UnitId,
                out var unit)
                ? unit.UnitNumber
                : "واحد غیرفعال یا یافت‌نشده";

            result.Add(new UnitChargeBalanceDto(
                unitCharge.Id,
                unitCharge.UnitId,
                unitNumber,
                unitCharge.Amount,
                paidAmount,
                unitCharge.Amount - paidAmount));
        }

        return result;
    }
}
