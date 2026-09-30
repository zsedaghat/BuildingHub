namespace BuildingHub.Modules.Community.Application.Floors.GetList;

public sealed record GetFloorsResponse(
    Guid Id,
    Guid BuildingId,
    string Name,
    int FloorNumber,
    int DisplayOrder);