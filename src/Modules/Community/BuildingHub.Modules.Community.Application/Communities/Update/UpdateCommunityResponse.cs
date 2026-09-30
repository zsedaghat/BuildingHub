namespace BuildingHub.Modules.Community.Application.Communities.Update;

public sealed record UpdateCommunityResponse(
    Guid Id,
    string Name,
    string? Description);