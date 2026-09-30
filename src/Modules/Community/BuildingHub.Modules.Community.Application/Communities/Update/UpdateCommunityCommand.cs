namespace BuildingHub.Modules.Community.Application.Communities.Update;

public sealed record UpdateCommunityCommand(
    Guid Id,
    string Name,
    string? Description);