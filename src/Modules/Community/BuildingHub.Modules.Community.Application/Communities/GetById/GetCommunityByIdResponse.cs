namespace BuildingHub.Modules.Community.Application.Communities.GetById;

public sealed record GetCommunityByIdResponse(
    Guid Id,
    string Name,
    string? Description);