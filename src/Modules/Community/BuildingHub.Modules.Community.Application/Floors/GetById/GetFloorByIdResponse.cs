namespace BuildingHub.Modules.Community.Application.Floors.GetById;

public sealed record GetFloorByIdResponse(
    Guid Id,
    Guid BuildingId,
    string Name,
    int FloorNumber,
    int DisplayOrder);