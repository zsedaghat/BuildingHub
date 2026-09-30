using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetByPersonId;

public sealed record GetUnitPersonsByPersonIdResponse(
    Guid Id,
    Guid UnitId,
    Guid PersonId,
    string UnitNumber,
    UnitPersonRole Role,
    bool IsPrimary,
    DateOnly? StartDate,
    DateOnly? EndDate);