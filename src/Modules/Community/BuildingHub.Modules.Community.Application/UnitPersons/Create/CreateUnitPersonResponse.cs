using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.Create;

public sealed record CreateUnitPersonResponse(
    Guid Id,
    Guid UnitId,
    Guid PersonId,
    UnitPersonRole Role,
    bool IsPrimary,
    DateOnly? StartDate,
    DateOnly? EndDate);