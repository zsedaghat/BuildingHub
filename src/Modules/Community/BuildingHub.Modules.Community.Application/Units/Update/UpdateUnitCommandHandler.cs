using BuildingHub.Modules.Community.Application.Abstractions;

namespace BuildingHub.Modules.Community.Application.Units.Update;

public sealed class UpdateUnitCommandHandler
{
    private readonly IUnitRepository _repository;

    public UpdateUnitCommandHandler(IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateUnitResponse?> Handle(
        UpdateUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        var unit = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (unit is null)
            return null;

        var unitNumber = command.UnitNumber?.Trim();

        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("شماره واحد الزامی است.");

        if (await _repository.ExistsAsync(
            unit.BuildingId,
            unitNumber,
            unit.Id,
            cancellationToken))
        {
            throw new InvalidOperationException(
                "این شماره واحد قبلاً برای این ساختمان ثبت شده است. لطفاً شماره دیگری انتخاب کنید.");
        }

        unit.Update(
            unitNumber,
            command.Type,
            command.Area);

        await _repository.UpdateAsync(
            unit,
            cancellationToken);

        return new UpdateUnitResponse(
            unit.Id,
            unit.BuildingId,
            unit.FloorId,
            unit.UnitNumber,
            unit.Type,
            unit.Area,
            unit.IsActive);
    }
}