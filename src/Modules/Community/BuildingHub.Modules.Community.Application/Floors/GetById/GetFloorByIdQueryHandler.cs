using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Floors.GetById;

public sealed class GetFloorByIdQueryHandler
{
    private readonly IFloorRepository _repository;

    public GetFloorByIdQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetFloorByIdResponse?> Handle(
        GetFloorByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var floor = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (floor is null)
            return null;

        return new GetFloorByIdResponse(
            floor.Id,
            floor.BuildingId,
            floor.Name,
            floor.FloorNumber,
            floor.DisplayOrder);
    }
}