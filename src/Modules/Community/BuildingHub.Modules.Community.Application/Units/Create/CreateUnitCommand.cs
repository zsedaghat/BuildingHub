using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.Create;

public sealed record CreateUnitCommand(
    Guid BuildingId,
    Guid? FloorId,
    string UnitNumber,
    UnitType Type,
    decimal? Area);