using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Floors.Delete;

public sealed class DeleteFloorCommandHandler
{
    private readonly IFloorRepository _repository;

    public DeleteFloorCommandHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteFloorCommand command,
        CancellationToken cancellationToken = default)
    {
        var floor = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (floor is null)
            return false;

        await _repository.DeleteAsync(
            floor,
            cancellationToken);

        return true;
    }
}