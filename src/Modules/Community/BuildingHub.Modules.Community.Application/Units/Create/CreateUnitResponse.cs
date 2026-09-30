using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.Create;

public sealed record CreateUnitResponse(
    Guid Id,
    Guid BuildingId,
    Guid? FloorId,
    string UnitNumber,
    UnitType Type,
    decimal? Area,
    bool IsActive);