using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.UnitPersons.GetByUnitId;

public sealed record GetUnitPersonsByUnitIdResponse(
    Guid Id,
    Guid UnitId,
    Guid PersonId,
    string FirstName,
    string LastName,
    string Mobile,
    string? Email,
    UnitPersonRole Role,
    bool IsPrimary,
    DateOnly? StartDate,
    DateOnly? EndDate);