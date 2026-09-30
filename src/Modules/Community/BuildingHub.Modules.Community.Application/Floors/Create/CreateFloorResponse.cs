namespace BuildingHub.Modules.Community.Application.Floors.Create;

public sealed record CreateFloorResponse(
    Guid Id,
    Guid BuildingId,
    string Name,
    int FloorNumber,
    int DisplayOrder);