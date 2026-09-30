using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Buildings.Update;

public sealed class UpdateBuildingCommandHandler
{
    private readonly IBuildingRepository _repository;

    public UpdateBuildingCommandHandler(
        IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateBuildingResponse?> Handle(
        UpdateBuildingCommand command,
        CancellationToken cancellationToken = default)
    {
        var building = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (building is null)
            return null;

        building.Update(
            command.Name,
            command.Description,
            command.DisplayOrder);

        await _repository.UpdateAsync(
            building,
            cancellationToken);

        return new UpdateBuildingResponse(
            building.Id,
            building.CommunityId,
            building.Name,
            building.Description,
            building.DisplayOrder);
    }
}