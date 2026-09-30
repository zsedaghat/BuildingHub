using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Floors.GetList;

public sealed class GetFloorsQueryHandler
{
    private readonly IFloorRepository _repository;

    public GetFloorsQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetFloorsResponse>> Handle(
        GetFloorsQuery query,
        CancellationToken cancellationToken = default)
    {
        var floors = await _repository.GetAllAsync(cancellationToken);

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