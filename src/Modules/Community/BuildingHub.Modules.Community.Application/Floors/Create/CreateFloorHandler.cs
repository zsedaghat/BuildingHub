using BuildingHub.Modules.Community.Application.Abstractions;

using FloorEntity =
    BuildingHub.Modules.Community.Domain.Entities.Floor;

namespace BuildingHub.Modules.Community.Application.Floors.Create;

public sealed class CreateFloorCommandHandler
{
    private readonly IFloorRepository _repository;

    public CreateFloorCommandHandler(
        IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateFloorResponse> Handle(
        CreateFloorCommand command,
        CancellationToken cancellationToken = default)
    {
        var floorNumberExists = await _repository.ExistsAsync(
            command.BuildingId,
            command.FloorNumber,
            cancellationToken: cancellationToken);

        if (floorNumberExists)
        {
            throw new InvalidOperationException(
                "این شماره طبقه قبلاً برای این ساختمان ثبت شده است. لطفاً شماره دیگری انتخاب کنید.");
        }

        var floor = FloorEntity.Create(
            command.BuildingId,
            command.Name,
            command.FloorNumber,
            command.DisplayOrder);

        await _repository.AddAsync(
            floor,
            cancellationToken);

        return new CreateFloorResponse(
            floor.Id,
            floor.BuildingId,
            floor.Name,
            floor.FloorNumber,
            floor.DisplayOrder);
    }
}
