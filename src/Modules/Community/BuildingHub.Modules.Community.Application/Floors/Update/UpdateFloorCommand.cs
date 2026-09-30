namespace BuildingHub.Modules.Community.Application.Floors.Update;

public sealed record UpdateFloorCommand(
    Guid Id,
    string Name,
    int FloorNumber,
    int DisplayOrder);