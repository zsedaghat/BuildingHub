namespace BuildingHub.Modules.Community.Application.Buildings.GetById;

public sealed record GetBuildingByIdResponse(
    Guid Id,
    Guid CommunityId,
    string Name,
    string? Description,
    int DisplayOrder);