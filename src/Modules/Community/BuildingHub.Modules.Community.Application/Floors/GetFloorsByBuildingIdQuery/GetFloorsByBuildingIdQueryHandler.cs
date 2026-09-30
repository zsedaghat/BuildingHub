using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Application.Floors.GetList;

namespace BuildingHub.Modules.Community.Application.Floors.GetByBuildingId;

public sealed class GetFloorsByBuildingIdQueryHandler
{
    private readonly IFloorRepository _repository;

    public GetFloorsByBuildingIdQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetFloorsResponse>> Handle(
        GetFloorsByBuildingIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var floors = await _repository.GetByBuildingIdAsync(
            query.BuildingId,
            cancellationToken);

        return floors
            .Select(x => new GetFloorsResponse(
                x.Id,
                x.BuildingId,
                x.Name,
                x.FloorNumber,
                x.DisplayOrder))
            .ToList();
    }
}