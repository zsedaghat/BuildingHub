using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetList;

public sealed record GetUnitPersonsResponse(
    Guid Id,
    Guid UnitId,
    Guid PersonId,
    UnitPersonRole Role,
    bool IsPrimary,
    DateOnly? StartDate,
    DateOnly? EndDate);