using BuildingHub.Modules.Community.Application.Abstractions;

using BuildingEntity =
    BuildingHub.Modules.Community.Domain.Entities.Building;

namespace BuildingHub.Modules.Community.Application.Buildings.Create;

public sealed class CreateBuildingCommandHandler
{
    private readonly IBuildingRepository _repository;

    public CreateBuildingCommandHandler(
        IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateBuildingResponse> Handle(
        CreateBuildingCommand command,
        CancellationToken cancellationToken = default)
    {
        var building = BuildingEntity.Create(
            command.CommunityId,
            command.Name,
            command.Description,
            command.DisplayOrder);

        await _repository.AddAsync(
            building,
            cancellationToken);

        return new CreateBuildingResponse(
            building.Id,
            building.CommunityId,
            building.Name,
            building.Description,
            building.DisplayOrder);
    }
}