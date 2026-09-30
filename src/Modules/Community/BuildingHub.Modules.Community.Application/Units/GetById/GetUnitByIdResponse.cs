using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.GetById;

public sealed record GetUnitByIdResponse(
    Guid Id,
    Guid BuildingId,
    Guid? FloorId,
    string UnitNumber,
    UnitType Type,
    decimal? Area,
    bool IsActive);