namespace BuildingHub.Modules.Community.Application.Communities.GetList;

public sealed record GetCommunityResponse(
    Guid Id,
    string Name,
    string? Description);