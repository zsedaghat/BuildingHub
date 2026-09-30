using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.Update;

public sealed record UpdateUnitCommand(
    Guid Id,
    string UnitNumber,
    UnitType Type,
    decimal? Area);