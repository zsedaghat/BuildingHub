using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Units.GetList;

namespace BuildingHub.Modules.Community.Application.Units.GetByBuildingId;

public sealed class GetUnitsByBuildingIdQueryHandler
{
    private readonly IUnitRepository _repository;

    public GetUnitsByBuildingIdQueryHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetUnitsResponse>> Handle(
        GetUnitsByBuildingIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var units = await _repository.GetByBuildingIdAsync(
            query.BuildingId,
            cancellationToken);

        return units
            .Select(x => new GetUnitsResponse(
                x.Id,
                x.BuildingId,
                x.FloorId,
                x.UnitNumber,
                x.Type,
                x.Area,
                x.IsActive))
            .ToList();
    }
}