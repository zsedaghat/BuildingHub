using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Buildings.GetList;

public sealed class GetBuildingsByCommunityIdQueryHandler
{
    private readonly IBuildingRepository _repository;

    public GetBuildingsByCommunityIdQueryHandler(
        IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetBuildingResponse>> Handle(
        GetBuildingsByCommunityIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var buildings = await _repository.GetByCommunityIdAsync(
            query.CommunityId,
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