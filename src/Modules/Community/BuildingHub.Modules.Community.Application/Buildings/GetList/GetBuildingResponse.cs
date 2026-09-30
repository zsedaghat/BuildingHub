namespace BuildingHub.Modules.Community.Application.Buildings.GetList;

public sealed record GetBuildingResponse(
    Guid Id,
    Guid CommunityId,
    string Name,
    string? Description,
    int DisplayOrder);