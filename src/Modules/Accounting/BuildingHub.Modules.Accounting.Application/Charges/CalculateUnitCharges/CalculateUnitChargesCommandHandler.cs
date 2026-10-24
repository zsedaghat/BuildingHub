
using BuildingHub.Modules.Accounting.Application.Abstractions;
using BuildingHub.Modules.Accounting.Domain.Entities;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.CalculateUnitCharges;

public sealed class CalculateUnitChargesCommandHandler
    : IRequestHandler<CalculateUnitChargesCommand, int>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IUnitChargeRepository _unitChargeRepository;
    private readonly IBuildingUnitProvider _unitProvider;

    public CalculateUnitChargesCommandHandler(
        IChargeRepository chargeRepository,
        IUnitChargeRepository unitChargeRepository,
        IBuildingUnitProvider unitProvider)
    {
        _chargeRepository = chargeRepository;
        _unitChargeRepository = unitChargeRepository;
        _unitProvider = unitProvider;
    }

    public async Task<int> Handle(
        CalculateUnitChargesCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ChargeId == Guid.Empty)
            throw new ArgumentException("ChargeId is required.");

        var charge = await _chargeRepository.GetByIdAsync(
            request.ChargeId,
            cancellationToken);

        if (charge is null)
            throw new InvalidOperationException("Charge was not found.");

        var existingCharges =
            await _unitChargeRepository.GetByChargeIdAsync(
                charge.Id,
                cancellationToken);

        if (existingCharges.Count > 0)
        {
            throw new InvalidOperationException(
                "Unit charges have already been calculated for this charge.");
        }

        if (charge.CalculationMethod == ChargeCalculationMethod.Custom)
        {
            throw new NotSupportedException(
                "Custom calculation requires manually entered amounts.");
        }

        var units = await _unitProvider.GetActiveUnitsAsync(
            charge.BuildingId,
            cancellationToken);

        if (units.Count == 0)
            throw new InvalidOperationException(
                "No active units were found for this building.");

        var totalArea = units.Sum(x => x.Area ?? 0m);
        var totalPeople = units.Sum(x => x.ResidentCount);

        if (charge.CalculationMethod == ChargeCalculationMethod.ByArea &&
            (units.Any(x => !x.Area.HasValue || x.Area.Value <= 0) ||
             totalArea <= 0))
        {
            throw new InvalidOperationException(
                "All units must have a valid area for area-based calculation.");
        }

        if (charge.CalculationMethod == ChargeCalculationMethod.ByPerson &&
            totalPeople <= 0)
        {
            throw new InvalidOperationException(
                "At least one resident is required for person-based calculation.");
        }

        if (charge.CalculationMethod == ChargeCalculationMethod.ByAreaAndPerson &&
            (units.Any(x => !x.Area.HasValue || x.Area.Value <= 0) ||
             totalArea <= 0 || totalPeople <= 0))
        {
            throw new InvalidOperationException(
                "Valid areas and resident counts are required for combined calculation.");
        }

        decimal GetWeight(BuildingUnitInfo unit)
        {
            return charge.CalculationMethod switch
            {
                ChargeCalculationMethod.Equal => 1m,

                ChargeCalculationMethod.ByArea =>
                    unit.Area!.Value,

                ChargeCalculationMethod.ByPerson =>
                    unit.ResidentCount,

                ChargeCalculationMethod.ByAreaAndPerson =>
                    (unit.Area!.Value / totalArea +
                     unit.ResidentCount / (decimal)totalPeople) / 2m,

                _ => throw new NotSupportedException(
                    "Unsupported calculation method.")
            };
        }

        var weights = units.Select(GetWeight).ToList();
        var totalWeight = weights.Sum();

        if (totalWeight <= 0)
            throw new InvalidOperationException(
                "The total calculation weight must be greater than zero.");

        decimal allocated = 0m;
        
    
        var unitChargesToCreate = new List<UnitCharge>();

        for (var i = 0; i < units.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            decimal amount;

            if (i == units.Count - 1)
            {
                // The final unit receives the rounding remainder.
                amount = charge.TotalAmount - allocated;
            }
            else
            {
                amount = decimal.Round(
                    charge.TotalAmount * weights[i] / totalWeight,
                    2,
                    MidpointRounding.ToZero);

                allocated += amount;
            }

            var unitCharge = UnitCharge.Create(
                charge.Id,
                units[i].UnitId,
                amount);

            unitChargesToCreate.Add(unitCharge);
        }

        await _unitChargeRepository.AddRangeAsync(
            unitChargesToCreate,
            cancellationToken);

        return unitChargesToCreate.Count;

    }
}
