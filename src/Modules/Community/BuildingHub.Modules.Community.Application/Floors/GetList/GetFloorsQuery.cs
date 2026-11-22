namespace BuildingHub.Modules.Community.Application.Floors.GetList;

public sealed record GetFloorsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? BuildingId = null);
