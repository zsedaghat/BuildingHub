namespace BuildingHub.Modules.Community.Application.Floors.Create;

public sealed record CreateFloorCommand(
    Guid BuildingId,
    string Name,
    int FloorNumber,
    int DisplayOrder);