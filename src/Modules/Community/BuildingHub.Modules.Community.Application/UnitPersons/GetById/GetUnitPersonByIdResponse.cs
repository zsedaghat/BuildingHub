using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetById;

public sealed record GetUnitPersonByIdResponse(
    Guid Id,
    Guid UnitId,
    Guid PersonId,
    UnitPersonRole Role,
    bool IsPrimary,
    DateOnly? StartDate,
    DateOnly? EndDate);