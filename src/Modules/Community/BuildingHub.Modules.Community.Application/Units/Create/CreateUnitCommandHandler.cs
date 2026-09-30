using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.Create;

public sealed class CreateUnitCommandHandler
{
    private readonly IUnitRepository _repository;

    public CreateUnitCommandHandler(
        IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateUnitResponse> Handle(
        CreateUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = Unit.Create(
            command.BuildingId,
            command.FloorId,
            command.UnitNumber,
            command.Type,
            command.Area);

        await _repository.AddAsync(
            unit,
            cancellationToken);

        return new CreateUnitResponse(
            unit.Id,
            unit.BuildingId,
            unit.FloorId,
            unit.UnitNumber,
            unit.Type,
            unit.Area,
            unit.IsActive);
    }
}