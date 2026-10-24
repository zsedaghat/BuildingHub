
using MediatR;

namespace BuildingHub.Modules.Community.Application.Units.GetActiveByBuildingId;

public sealed record GetActiveUnitsByBuildingIdQuery(Guid BuildingId)
    : IRequest<IReadOnlyList<ActiveUnitDto>>;

public sealed record ActiveUnitDto(
    Guid UnitId,
    string UnitNumber,
    decimal? Area
);
