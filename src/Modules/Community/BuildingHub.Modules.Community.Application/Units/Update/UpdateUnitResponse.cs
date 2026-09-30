using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.Update;

public sealed record UpdateUnitResponse(
    Guid Id,
    Guid BuildingId,
    Guid? FloorId,
    string UnitNumber,
    UnitType Type,
    decimal? Area,
    bool IsActive);