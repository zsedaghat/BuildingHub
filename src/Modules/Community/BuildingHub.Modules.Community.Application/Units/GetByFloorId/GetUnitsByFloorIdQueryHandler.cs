using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Units.GetList;

namespace BuildingHub.Modules.Community.Application.Units.GetByFloorId;

public sealed class GetUnitsByFloorIdQueryHandler
{
    private readonly IUnitRepository _repository;

    public GetUnitsByFloorIdQueryHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetUnitsResponse>> Handle(
        GetUnitsByFloorIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var units = await _repository.GetByFloorIdAsync(
            query.FloorId,
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