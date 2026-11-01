
using BuildingHub.Modules.Accounting.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Accounting.Application.Charges.GetBuildingUnitBalances;

public sealed class GetBuildingUnitBalancesQueryHandler
    : IRequestHandler<GetBuildingUnitBalancesQuery,
        IReadOnlyList<BuildingUnitBalanceDto>>
{
    private readonly IUnitChargeRepository _unitChargeRepository;
    private readonly IBuildingUnitProvider _unitProvider;

    public GetBuildingUnitBalancesQueryHandler(
        IUnitChargeRepository unitChargeRepository,
        IBuildingUnitProvider unitProvider)
    {
        _unitChargeRepository = unitChargeRepository;
        _unitProvider = unitProvider;
    }

    public async Task<IReadOnlyList<BuildingUnitBalanceDto>> Handle(
        GetBuildingUnitBalancesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.BuildingId == Guid.Empty)
        {
            throw new ArgumentException(
                "BuildingId is required.");
        }

        var units = await _unitProvider.GetActiveUnitsAsync(
            request.BuildingId,
            cancellationToken);

        var summaries =
            await _unitChargeRepository.GetBuildingBalanceSummariesAsync(
                request.BuildingId,
                cancellationToken);

        var summariesByUnitId = summaries.ToDictionary(
            x => x.UnitId);

        var result = new List<BuildingUnitBalanceDto>();

        foreach (var unit in units)
        {
            if (summariesByUnitId.TryGetValue(
                unit.UnitId,
                out var summary))
            {
                result.Add(new BuildingUnitBalanceDto(
                    unit.UnitId,
                    unit.UnitNumber,
                    summary.TotalChargeAmount,
                    summary.TotalPaidAmount,
                    summary.TotalChargeAmount - summary.TotalPaidAmount));
            }
            else
            {
                result.Add(new BuildingUnitBalanceDto(
                    unit.UnitId,
                    unit.UnitNumber,
                    0m,
                    0m,
                    0m));
            }
        }

        return result;
    }
}
