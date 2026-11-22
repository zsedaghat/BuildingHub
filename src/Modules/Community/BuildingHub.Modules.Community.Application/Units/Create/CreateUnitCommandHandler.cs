using BuildingHub.Modules.Community.Application.Abstractions;
using BuildingHub.Modules.Community.Domain.Entities;

namespace BuildingHub.Modules.Community.Application.Units.Create;

public sealed class CreateUnitCommandHandler
{
    private readonly IUnitRepository _repository;

    public CreateUnitCommandHandler(
        IUnitRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateUnitResponse> Handle(
        CreateUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        var unitNumber = command.UnitNumber?.Trim();

        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new ArgumentException("شماره واحد الزامی است.");

        if (command.FloorId.HasValue &&
            !await _repository.FloorBelongsToBuildingAsync(
                command.FloorId.Value,
                command.BuildingId,
                cancellationToken))
        {
            throw new ArgumentException("طبقه انتخاب‌شده متعلق به ساختمان انتخاب‌شده نیست.");
        }

        if (await _repository.ExistsAsync(
            command.BuildingId,
            unitNumber,
            cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                "این شماره واحد قبلاً برای این ساختمان ثبت شده است. لطفاً شماره دیگری انتخاب کنید.");
        }

        var unit = Unit.Create(
            command.BuildingId,
            command.FloorId,
            unitNumber,
            command.Type,
            command.Area);

        await _repository.AddAsync(
            unit,
            cancellationToken);

        return new CreateUnitResponse(
            unit.Id,
            unit.BuildingId,
            unit.FloorId,
            unit.UnitNumber,
            unit.Type,
            unit.Area,
            unit.IsActive);
    }
}