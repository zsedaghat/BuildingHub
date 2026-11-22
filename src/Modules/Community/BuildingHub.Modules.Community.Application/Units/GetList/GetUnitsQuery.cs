namespace BuildingHub.Modules.Community.Application.Units.GetList;

public sealed record GetUnitsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? BuildingId = null,
    Guid? FloorId = null);
