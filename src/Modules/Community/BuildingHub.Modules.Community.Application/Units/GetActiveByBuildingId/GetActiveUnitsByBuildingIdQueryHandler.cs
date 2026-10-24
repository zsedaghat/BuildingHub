
using BuildingHub.Modules.Community.Application.Abstractions;
using MediatR;

namespace BuildingHub.Modules.Community.Application.Units.GetActiveByBuildingId;

public sealed class GetActiveUnitsByBuildingIdQueryHandler
    : IRequestHandler<GetActiveUnitsByBuildingIdQuery, IReadOnlyList<ActiveUnitDto>>
{
    private readonly IUnitRepository _unitRepository;

    public GetActiveUnitsByBuildingIdQueryHandler(
        IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<IReadOnlyList<ActiveUnitDto>> Handle(
        GetActiveUnitsByBuildingIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.BuildingId == Guid.Empty)
            throw new ArgumentException("BuildingId is required.");

        var units = await _unitRepository.GetByBuildingIdAsync(
            request.BuildingId,
            cancellationToken);

        return units
            .Where(unit => unit.IsActive)
            .Select(unit => new ActiveUnitDto(
                unit.Id,
                unit.UnitNumber,
                unit.Area))
            .ToList();
    }
}
