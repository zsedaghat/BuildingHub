using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.Update;

public sealed record UpdateUnitPersonResponse(
    Guid Id,
    Guid UnitId,
    Guid PersonId,
    UnitPersonRole Role,
    bool IsPrimary,
    DateOnly? StartDate,
    DateOnly? EndDate);