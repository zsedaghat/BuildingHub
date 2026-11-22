using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Floors.Update;

public sealed class UpdateFloorCommandHandler
{
    private readonly IFloorRepository _repository;

    public UpdateFloorCommandHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateFloorResponse?> Handle(
        UpdateFloorCommand command,
        CancellationToken cancellationToken = default)
    {
        var floor = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (floor is null)
            return null;

        var floorNumberExists = await _repository.ExistsAsync(
            floor.BuildingId,
            command.FloorNumber,
            floor.Id,
            cancellationToken);

        if (floorNumberExists)
        {
            throw new InvalidOperationException(
                "این شماره طبقه قبلاً برای این ساختمان ثبت شده است. لطفاً شماره دیگری انتخاب کنید.");
        }

        floor.Update(
            command.Name,
            command.FloorNumber,
            command.DisplayOrder);

        await _repository.UpdateAsync(
            floor,
            cancellationToken);

        return new UpdateFloorResponse(
            floor.Id,
            floor.BuildingId,
            floor.Name,
            floor.FloorNumber,
            floor.DisplayOrder);
    }
}
