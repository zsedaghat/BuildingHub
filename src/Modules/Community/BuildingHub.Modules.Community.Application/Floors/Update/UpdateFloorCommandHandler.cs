using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Floors.Update;

public sealed class UpdateFloorCommandHandler
{
    private readonly IFloorRepository _repository;

    public UpdateFloorCommandHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateFloorResponse?> Handle(
        UpdateFloorCommand command,
        CancellationToken cancellationToken = default)
    {
        var floor = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (floor is null)
            return null;

        floor.Update(
            command.Name,
            command.FloorNumber,
            command.DisplayOrder);

        await _repository.UpdateAsync(
            floor,
            cancellationToken);

        return new UpdateFloorResponse(
            floor.Id,
            floor.BuildingId,
            floor.Name,
            floor.FloorNumber,
            floor.DisplayOrder);
    }
}