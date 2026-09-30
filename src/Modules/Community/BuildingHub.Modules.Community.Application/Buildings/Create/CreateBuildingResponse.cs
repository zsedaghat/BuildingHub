namespace BuildingHub.Modules.Community.Application.Buildings.Create;

public sealed record CreateBuildingResponse(
    Guid Id,
    Guid CommunityId,
    string Name,
    string? Description,
    int DisplayOrder);