
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.CalculateCustomUnitCharges;

public sealed class CalculateCustomUnitChargesCommandHandler
    : IRequestHandler<CalculateCustomUnitChargesCommand, int>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IUnitChargeRepository _unitChargeRepository;
    private readonly IBuildingUnitProvider _unitProvider;

    public CalculateCustomUnitChargesCommandHandler(
        IChargeRepository chargeRepository,
        IUnitChargeRepository unitChargeRepository,
        IBuildingUnitProvider unitProvider)
    {
        _chargeRepository = chargeRepository;
        _unitChargeRepository = unitChargeRepository;
        _unitProvider = unitProvider;
    }

    public async Task<int> Handle(
        CalculateCustomUnitChargesCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ChargeId == Guid.Empty)
            throw new ArgumentException("ChargeId is required.");

        if (request.Units is null || request.Units.Count == 0)
            throw new ArgumentException("Units list cannot be empty.");

        var charge = await _chargeRepository.GetByIdAsync(
            request.ChargeId,
            cancellationToken);

        if (charge is null)
            throw new InvalidOperationException("Charge was not found.");

        if (charge.CalculationMethod != ChargeCalculationMethod.Custom)
            throw new InvalidOperationException(
                "This charge does not use Custom calculation.");

        var existing = await _unitChargeRepository.GetByChargeIdAsync(
            charge.Id,
            cancellationToken);

        if (existing.Count > 0)
            throw new InvalidOperationException(
                "Unit charges have already been calculated.");

        if (request.Units.Any(x => x.UnitId == Guid.Empty || x.Amount < 0))
            throw new ArgumentException(
                "Invalid unit ID or negative amount.");

        if (request.Units.Select(x => x.UnitId).Distinct().Count()
            != request.Units.Count)
            throw new ArgumentException(
                "Duplicate units are not allowed.");

        var activeUnits = await _unitProvider.GetActiveUnitsAsync(
            charge.BuildingId,
            cancellationToken);

        var activeUnitIds = activeUnits
            .Select(x => x.UnitId)
            .ToHashSet();

        if (request.Units.Any(x => !activeUnitIds.Contains(x.UnitId)))
            throw new InvalidOperationException(
                "One or more units do not belong to this building or are inactive.");

        var total = request.Units.Sum(x => x.Amount);

        if (total != charge.TotalAmount)
            throw new InvalidOperationException(
                "Sum of unit amounts must equal the charge total amount.");

        var unitCharges = request.Units
            .Select(x => UnitCharge.Create(
                charge.Id,
                x.UnitId,
                x.Amount))
            .ToList();

        await _unitChargeRepository.AddRangeAsync(
            unitCharges,
            cancellationToken);

        return unitCharges.Count;
    }
}
