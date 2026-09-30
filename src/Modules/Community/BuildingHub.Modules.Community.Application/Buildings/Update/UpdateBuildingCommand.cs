namespace BuildingHub.Modules.Community.Application.Buildings.Update;

public sealed record UpdateBuildingCommand(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder);