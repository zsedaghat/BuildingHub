using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Buildings.GetById;

public sealed class GetBuildingByIdQueryHandler
{
    private readonly IBuildingRepository _repository;

    public GetBuildingByIdQueryHandler(
        IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetBuildingByIdResponse?> Handle(
        GetBuildingByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var building = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (building is null)
            return null;

        return new GetBuildingByIdResponse(
            building.Id,
            building.CommunityId,
            building.Name,
            building.Description,
            building.DisplayOrder);
    }
}