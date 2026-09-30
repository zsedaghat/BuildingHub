namespace BuildingHub.Modules.Community.Application.Floors.Update;

public sealed record UpdateFloorResponse(
    Guid Id,
    Guid BuildingId,
    string Name,
    int FloorNumber,
    int DisplayOrder);