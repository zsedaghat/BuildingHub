using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.GetList;

public sealed record GetUnitsResponse(
    Guid Id,
    Guid BuildingId,
    Guid? FloorId,
    string UnitNumber,
    UnitType Type,
    decimal? Area,
    bool IsActive);