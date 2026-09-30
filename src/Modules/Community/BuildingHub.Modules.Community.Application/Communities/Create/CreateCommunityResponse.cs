namespace BuildingHub.Modules.Community.Application.Communities.Create;

public sealed record CreateCommunityResponse(
    Guid Id,
    string Name,
    string? Description);