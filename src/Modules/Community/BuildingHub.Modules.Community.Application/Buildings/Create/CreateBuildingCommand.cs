namespace BuildingHub.Modules.Community.Application.Buildings.Create;

public sealed record CreateBuildingCommand(
    Guid CommunityId,
    string Name,
    string? Description,
    int DisplayOrder);