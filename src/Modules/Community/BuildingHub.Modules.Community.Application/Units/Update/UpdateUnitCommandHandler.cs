using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Units.Update;

public sealed class UpdateUnitCommandHandler
{
    private readonly IUnitRepository _repository;

    public UpdateUnitCommandHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateUnitResponse?> Handle(
        UpdateUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (unit is null)
            return null;

        unit.Update(
            command.UnitNumber,
            command.Type,
            command.Area);

        await _repository.UpdateAsync(
            unit,
            cancellationToken);

        return new UpdateUnitResponse(
            unit.Id,
            unit.BuildingId,
            unit.FloorId,
            unit.UnitNumber,
            unit.Type,
            unit.Area,
            unit.IsActive);
    }
}