using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Buildings.Delete;

public sealed class DeleteBuildingCommandHandler
{
    private readonly IBuildingRepository _repository;

    public DeleteBuildingCommandHandler(
        IBuildingRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteBuildingCommand command,
        CancellationToken cancellationToken = default)
    {
        var building = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (building is null)
            return false;

        await _repository.DeleteAsync(
            building,
            cancellationToken);

        return true;
    }
}