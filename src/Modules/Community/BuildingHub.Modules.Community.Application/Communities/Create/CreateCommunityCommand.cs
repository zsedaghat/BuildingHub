namespace BuildingHub.Modules.Community.Application.Communities.Create;

public sealed record CreateCommunityCommand(
    string Name,
    string? Description);