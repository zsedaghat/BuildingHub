using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.Update;

public sealed record UpdateUnitPersonCommand(
    Guid Id,
    UnitPersonRole Role,
    bool IsPrimary,
   DateOnly? StartDate,
    DateOnly? EndDate);