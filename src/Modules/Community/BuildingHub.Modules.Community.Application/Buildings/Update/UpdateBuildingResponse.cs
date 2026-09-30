namespace BuildingHub.Modules.Community.Application.Buildings.Update;

public sealed record UpdateBuildingResponse(
    Guid Id,
    Guid CommunityId,
    string Name,
    string? Description,
    int DisplayOrder);