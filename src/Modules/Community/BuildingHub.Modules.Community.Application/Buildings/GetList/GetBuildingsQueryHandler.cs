using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Buildings.GetList;

public sealed class GetBuildingsQueryHandler
{
    private readonly IBuildingRepository _repository;

    public GetBuildingsQueryHandler(
        IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetBuildingResponse>> Handle(
        GetBuildingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var buildings = await _repository.GetAllAsync(
            cancellationToken);

        return buildings
            .Select(x => new GetBuildingResponse(
                x.Id,
                x.CommunityId,
                x.Name,
                x.Description,
                x.DisplayOrder))
            .ToList();
    }
}